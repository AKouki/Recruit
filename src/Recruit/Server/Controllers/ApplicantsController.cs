using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Server.Services.BlobService;
using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicantsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;

        public ApplicantsController(ApplicationDbContext db, IBlobService blobService)
        {
            _db = db;
            _blobService = blobService;
        }

        [HttpGet]
        public async Task<IEnumerable<Applicant>>? Get()
        {
            var applicants = await _db.Applicants
                .Include(a => a.Job!)
                    .ThenInclude(s => s.Stages)
                .Include(a => a.Interview)
                .Include(a => a.Education)
                .Include(a => a.Experience)
                .OrderByDescending(a => a.ApplyDate)
                .ToListAsync();

            return applicants;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var applicant = await _db.Applicants
                .Include(a => a.Job!)
                    .ThenInclude(s => s.Stages)
                .Include(a => a.Education)
                .Include(a => a.Experience)
                .Include(a => a.Resume)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
                return NotFound();

            return Ok(applicant);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusViewModel model)
        {
            var applicant = await _db.Applicants.FirstOrDefaultAsync(a => a.Id == model.ApplicantId);
            var newStage = await _db.Stages.FirstOrDefaultAsync(s => s.Id == model.StageId);

            if (applicant == null || newStage == null)
                return BadRequest();

            applicant.Stage = newStage;

            _db.Applicants.Update(applicant);
            await _db.SaveChangesAsync();

            return Ok(applicant);
        }

        [HttpPost("UpdateStatusAndOrder")]
        public async Task<IActionResult> UpdateStatusAndOrder([FromBody] UpdateOrderViewModel model)
        {
            // Get the target stage
            var stage = await _db.Stages.FirstOrDefaultAsync(s => s.Id == model.StageId);
            if (stage == null)
                return NotFound();

            // Get applicants to update stage and/or order
            var applicantIds = model.Items.Select(a => a.ApplicantId).ToList();
            var applicants = _db.Applicants.Where(a => applicantIds.Contains(a.Id)).ToList();

            // Perform update
            foreach (var applicant in applicants)
            {
                if (applicant.Stage != stage)
                    applicant.Stage = stage;

                var item = model.Items.FirstOrDefault(i => i.ApplicantId == applicant.Id);
                applicant.Order = item?.Position ?? 0;
            }

            _db.Applicants.UpdateRange(applicants);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("Copy")]
        public async Task<IActionResult> Copy([FromBody] MoveApplicantViewModel model)
        {
            var applicant = await _db.Applicants
                .Where(a => a.Id == model.ApplicantId)
                .Include(a => a.Resume)
                .Include(a => a.Education)
                .Include(a => a.Experience)
                .FirstOrDefaultAsync();

            var job = await _db.Jobs
                .Where(j => j.Id == model.JobId)
                .Include(j => j.Stages)
                .FirstOrDefaultAsync();

            var stage = job?.Stages?.FirstOrDefault(s => s.Id == model.StageId);

            if (applicant == null || job == null || stage == null)
                return NotFound();

            // Check if applicant already exists in target job
            var exists = await _db.Applicants.AnyAsync(a => a.Email == applicant.Email && a.JobId == job.Id);
            if (exists)
                return BadRequest();

            // Copy record
            var newApplicant = new Applicant()
            {
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                Headline = applicant.Headline,
                Summary = applicant.Summary,
                ProfilePhoto = applicant.ProfilePhoto,
                Email = applicant.Email,
                Phone = applicant.Phone,
                Address = applicant.Address,
                Skills = applicant.Skills,
                ApplyDate = applicant.ApplyDate,
                JobId = job.Id,
                Stage = stage,
                Education = new List<Education>(),
                Experience = new List<Experience>()
            };

            // Copy education
            foreach (var education in applicant.Education ?? Enumerable.Empty<Education>())
            {
                newApplicant.Education?.Add(new Education()
                {
                    School = education.School,
                    Degree = education.Degree,
                    StartDate = education.StartDate,
                    EndDate = education.EndDate
                });
            }

            // Copy experience
            foreach (var experience in applicant.Experience ?? Enumerable.Empty<Experience>())
            {
                newApplicant.Experience?.Add(new Experience()
                {
                    Title = experience.Title,
                    Company = experience.Company,
                    StartDate = experience.StartDate,
                    EndDate = experience.EndDate,
                    CurrentlyWorking = experience.CurrentlyWorking
                });
            }

            // Copy resume
            if (!string.IsNullOrEmpty(applicant.Resume?.FilePath))
            {
                var newBlobName = await _blobService.CopyBlobAsync(applicant.Resume.FilePath);
                newApplicant.Resume = new Attachment()
                {
                    FileName = applicant.Resume.FileName,
                    FilePath = newBlobName
                };
            }

            _db.Applicants.Add(newApplicant);
            await _db.SaveChangesAsync();

            return Ok(newApplicant);
        }

        [HttpPost("Move")]
        public async Task<IActionResult> Move([FromBody] MoveApplicantViewModel model)
        {
            var applicant = await _db.Applicants.FindAsync(model.ApplicantId);

            var job = await _db.Jobs
                .Where(j => j.Id == model.JobId)
                .Include(j => j.Stages)
                .Include(j => j.Applicants)
                .FirstOrDefaultAsync();

            var stage = job?.Stages?.FirstOrDefault(s => s.Id == model.StageId);

            if (applicant == null || job == null || stage == null)
                return NotFound();

            // Check if another applicant with same email exists in target job
            var exists = job.Applicants!.Any(a => a.Email == applicant.Email);
            if (exists)
                return BadRequest();

            applicant.JobId = job.Id;
            applicant.Stage = stage;

            _db.Applicants.Update(applicant);
            await _db.SaveChangesAsync();

            return Ok(applicant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var applicant = await _db.Applicants
                .Include(a => a.Resume)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
                return NotFound();

            // Delete resume and profile photo from Blob Storage
            if (!string.IsNullOrEmpty(applicant.Resume?.FilePath))
                await _blobService.DeleteResumeAsync(applicant.Resume.FilePath);

            if (!string.IsNullOrEmpty(applicant.ProfilePhoto))
                await _blobService.DeletePhotoAsync(applicant.ProfilePhoto);

            // Delete applicant from database
            _db.Applicants.Remove(applicant);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
