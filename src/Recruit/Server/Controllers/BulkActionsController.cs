using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Server.Helpers;
using Recruit.Server.Models;
using Recruit.Server.Services.BlobService;
using Recruit.Server.Services.EmailService;
using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BulkActionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IBlobService _blobService;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;


        public BulkActionsController(
            ApplicationDbContext db,
            IBlobService blobService,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _blobService = blobService;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost("CopyApplicantsToJob")]
        public async Task<IActionResult> CopyApplicantsToJob([FromBody] BulkMoveApplicantViewModel viewModel)
        {
            var applicants = await _db.Applicants
                .Where(a => viewModel.Applicants.Contains(a.Id))
                .Include(a => a.Resume)
                .ToListAsync();

            var job = await _db.Jobs.Where(j => j.Id == viewModel.JobId)
                .Include(j => j.Stages)
                .FirstOrDefaultAsync();

            var stage = job?.Stages?.FirstOrDefault(s => s.Id == viewModel.StageId);

            if (job == null || stage == null)
                return NotFound();

            var newApplicants = new List<Applicant>();
            int index = 0;
            foreach (var applicant in applicants)
            {
                // Check if applicant already exists in target job
                var exists = await _db.Applicants.AnyAsync(a => a.Email == applicant.Email && a.JobId == job.Id);
                if (!exists)
                {
                    var newApplicant = new Applicant()
                    {
                        FirstName = applicant.FirstName,
                        LastName = applicant.LastName,
                        Headline = applicant.Headline,
                        Summary = applicant.Summary,
                        Email = applicant.Email,
                        Phone = applicant.Phone,
                        ProfilePhoto = applicant.ProfilePhoto,
                        Address = applicant.Address,
                        Skills = applicant.Skills,
                        ApplyDate = DateTime.Now.AddSeconds(index++),
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

                    // Copy photo
                    if (!string.IsNullOrEmpty(applicant.ProfilePhoto))
                    {
                        var newBlobUri = await _blobService.CopyPhotoAsync(Path.GetFileName(applicant.ProfilePhoto));
                        if (!string.IsNullOrEmpty(newBlobUri))
                            newApplicant.ProfilePhoto = newBlobUri;
                    }

                    // Copy resume
                    if (!string.IsNullOrEmpty(applicant.Resume?.FilePath))
                    {
                        var newBlobUri = await _blobService.CopyResumeAsync(applicant.Resume.FilePath);
                        newApplicant.Resume = new Attachment()
                        {
                            FileName = applicant.Resume.FileName,
                            FilePath = Path.GetFileName(newBlobUri) // For resumes we want only the blob name
                        };
                    }

                    newApplicants.Add(newApplicant);
                }
            }

            _db.Applicants.AddRange(newApplicants);
            await _db.SaveChangesAsync();

            return Ok(newApplicants);
        }

        [HttpPost("MoveApplicantsToJob")]
        public async Task<IActionResult> MoveApplicantsToJob([FromBody] BulkMoveApplicantViewModel viewModel)
        {
            var applicantsToMove = await _db.Applicants
                .Where(a => viewModel.Applicants.Contains(a.Id))
                .Include(a => a.Resume)
                .ToListAsync();

            var job = await _db.Jobs.Where(j => j.Id == viewModel.JobId)
                .Include(j => j.Stages)
                .Include(j => j.Applicants)
                .FirstOrDefaultAsync();

            var stage = job?.Stages?.FirstOrDefault(s => s.Id == viewModel.StageId);

            if (job == null || stage == null)
                return NotFound();
            
            // Remove applicant from list if theres already another applicant with the same email in target job
            applicantsToMove.RemoveAll(a => job.Applicants!.Any(b => a.Email == b.Email));

            foreach (var applicant in applicantsToMove)
            {
                applicant.JobId = job.Id;
                applicant.Stage = stage;
            }

            _db.Applicants.UpdateRange(applicantsToMove);
            await _db.SaveChangesAsync();

            return Ok(applicantsToMove);
        }

        [HttpPost("DeleteApplicants")]
        public async Task<IActionResult> DeleteApplicants([FromBody] BulkActionViewModel viewModel)
        {
            var list = new List<int>();
            var applicantsToDelete = await _db.Applicants
                .Where(a => viewModel.Items.Contains(a.Id))
                .Include(a => a.Resume)
                .ToListAsync();

            // Delete resumes and profile photos from Blob Storage
            var resumesToDelete = applicantsToDelete.Select(a => a.Resume?.FilePath).ToList();
            if (resumesToDelete.Any())
                await _blobService.DeleteResumesAsync(resumesToDelete!);

            var photosToDelete = applicantsToDelete
                .Where(a => !string.IsNullOrEmpty(a.ProfilePhoto))
                .Select(a => a.ProfilePhoto)
                .ToList();
            if (photosToDelete.Any())
                await _blobService.DeletePhotosAsync(photosToDelete!);

            // Delete applicants from database
            _db.Applicants.RemoveRange(applicantsToDelete);
            await _db.SaveChangesAsync();

            return Ok(applicantsToDelete.Select(a => a.Id).ToList());
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] BulkSendEmailViewModel model)
        {
            var list = new List<int>();
            var applicants = await _db.Applicants
                .Where(a => model.ApplicantIds!.Contains(a.Id))
                .Include(a => a.Job)
                .ToListAsync();

            var sender = await _userManager.FindByEmailAsync(User.Identity!.Name);

            foreach (var applicant in applicants)
            {
                try
                {
                    // Send email
                    var htmlMessage = EmailHelper.GenerateHtmlMessage(model.Body, applicant, sender);
                    await _emailService.SendEmailAsync(applicant.Email!, model.Subject!, htmlMessage);

                    // Store email in database
                    var sentEmail = new EmailItem()
                    {
                        Sender = sender,
                        Receiver = applicant,
                        Subject = model.Subject?.Trim(),
                        Body = htmlMessage,
                        SentDate = DateTime.Now
                    };

                    _db.Emails.Add(sentEmail);
                    await _db.SaveChangesAsync();
                }
                catch (Exception)
                {
                }
            }

            return Ok();
        }
    }
}
