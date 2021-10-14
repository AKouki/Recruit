using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobOpeningsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public JobOpeningsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<Job>>? Get()
        {
            var jobs = await _db.Jobs
                .Where(j => j.Published == true)
                .OrderByDescending(j => j.PostDate)
                .ToListAsync();

            return jobs;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var job = _db.Jobs
                .Where(j => j.Published == true && j.Id == id)
                .FirstOrDefault();

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> Apply([FromForm] ApplicationModel model)
        {
            var job = _db.Jobs
                .Include(j => j.Stages)
                .FirstOrDefault(j => j.Id == model.JobId);

            if (job == null)
                return BadRequest();

            if (!ValidateResume(model.Resume))
                return BadRequest();

            if (model.Photo != null && !ValidatePhoto(model.Photo))
                return BadRequest();

            var newApplicant = new Applicant()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Email = model.Email,
                Phone = model.Phone,
                Headline = model.Headline,
                Skills = model.Skills,
                Summary = model.Summary,
                ApplyDate = DateTime.Now,
                Job = job,
                Stage = job.Stages?.FirstOrDefault(s => s.Name == "Screen"),
            };

            //TODO: Save photo/resume

            newApplicant.Resume = new Attachment() { FileName = model.Resume?.FileName, FilePath = "FilePath" };

            _db.Applicants.Add(newApplicant);
            await _db.SaveChangesAsync();

            return Ok();
        }

        private bool ValidateFile(IFormFile? file, int maxAllowedFileSize, string allowedExtensionsStr)
        {
            if (file != null)
            {
                string[] AllowedExtenstions = allowedExtensionsStr.Split(',');

                if (file.Length > maxAllowedFileSize)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !AllowedExtenstions.Any(s => s.Contains(extension)))
                    return false;

                if (file.FileName.Length > 255)
                    return false;

                return true;
            }

            return false;
        }
        private bool ValidateResume(IFormFile? file)
        {
            return ValidateFile(file, 2 * 1024 * 1024, ".docx,.pdf");
        }
        private bool ValidatePhoto(IFormFile? file)
        {
            return ValidateFile(file, 1 * 1024 * 1024, ".jpg,.jpeg");
        }
    }
}
