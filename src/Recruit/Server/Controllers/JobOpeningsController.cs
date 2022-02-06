using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Server.Services.BlobService;
using Recruit.Shared;
using Recruit.Shared.Validators;
using Recruit.Shared.ViewModels;
using System.Text.Json;

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
        private readonly IConfiguration _configuration;

        public JobOpeningsController(ApplicationDbContext db,
            IFileValidator fileValidator,
            IWebHostEnvironment env,
            IBlobService blobService, 
            IConfiguration configuration)
        {
            _db = db;
            _fileValidator = fileValidator;
            _blobService = blobService;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IEnumerable<Job>> Get()
        {
            var jobs = await _db.Jobs
                .Where(j => j.Published == true)
                .Include(j => j.Department)
                .OrderByDescending(j => j.PostDate)
                .ToListAsync();

            return jobs;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var job = await _db.Jobs
                .Where(j => j.Id == id && j.Published == true)
                .Include(j => j.Department)
                .FirstOrDefaultAsync();

            if (job == null)
                return NotFound();

            return Ok(job);
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> Apply([FromForm] ApplicationModel model)
        {
            var job = await _db.Jobs
                .Where(j => j.Id == model.JobId && j.Published == true)
                .Include(j => j.Stages)
                .FirstOrDefaultAsync();

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
                Stage = job.Stages?.FirstOrDefault(s => s.Name == "Screen")
            };

            if (!string.IsNullOrEmpty(model.EducationJson))
            {
                var educations = JsonSerializer.Deserialize<List<Education>>(model.EducationJson);
                newApplicant.Education = educations?.Take(5).ToList();
            }

            if (!string.IsNullOrEmpty(model.ExperienceJson))
            {
                var experiences = JsonSerializer.Deserialize<List<Experience>>(model.ExperienceJson);
                newApplicant.Experience = experiences?.Take(5).ToList();
            }

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
                    string storageAccountName = _configuration["AzureBlobStorageSettings:StorageAccountName"];
                    newApplicant.ProfilePhoto = _env.IsDevelopment() ?
                        $"http://127.0.0.1:10000/devstoreaccount1/photos/{blobName}" :
                        $"https://{storageAccountName}.blob.core.windows.net/photos/{blobName}";
                }
            }

            _db.Applicants.Add(newApplicant);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
