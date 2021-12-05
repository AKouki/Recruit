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
