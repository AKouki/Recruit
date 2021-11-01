using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Server.Services.BlobService;
using Recruit.Shared;
using Recruit.Shared.Validators;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobOpeningsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IFileValidator _fileValidator;
        private readonly IWebHostEnvironment _env;
        private readonly IBlobService _blobService;

        public JobOpeningsController(ApplicationDbContext db,
            IFileValidator fileValidator,
            IWebHostEnvironment env,
            IBlobService blobService)
        {
            _db = db;
            _fileValidator = fileValidator;
            _blobService = blobService;
            _env = env;
        }

        [HttpGet]
        public async Task<IEnumerable<Job>> Get()
        {
            var jobs = await _db.Jobs
                .Where(j => j.Published == true)
                .OrderByDescending(j => j.PostDate)
                .ToListAsync();

            return jobs;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _db.Jobs
                .FirstOrDefaultAsync(j => j.Published == true && j.Id == id);

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> Apply([FromForm] ApplicationModel model)
        {
            var job = await _db.Jobs
                .Include(j => j.Stages)
                .FirstOrDefaultAsync(j => j.Id == model.JobId);

            if (job == null)
                return BadRequest();

            if (!_fileValidator.IsValidResume(model.Resume))
                return BadRequest();

            if (model.Photo != null && !_fileValidator.IsValidPhoto(model.Photo))
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

            // Upload resume
            if (model.Resume != null)
            {
                var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(model.Resume.FileName);
                var uploaded = await _blobService.UploadResumeAsync(model.Resume, blobName);
                if (uploaded)
                    newApplicant.Resume = new Attachment() { FileName = model.Resume?.FileName, FilePath = blobName };
            }

            // Upload photo
            if (model.Photo != null)
            {
                var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(model.Photo.FileName);
                var uploaded = await _blobService.UploadPhotoAsync(model.Photo, blobName);
                if (uploaded)
                {
                    newApplicant.ProfilePhoto = _env.IsDevelopment() ?
                        $"http://127.0.0.1:10000/devstoreaccount1/photos/{blobName}" :
                        $"https://<storage_account_name>.blob.core.windows.net/photos/{blobName}";
                }
            }

            _db.Applicants.Add(newApplicant);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
