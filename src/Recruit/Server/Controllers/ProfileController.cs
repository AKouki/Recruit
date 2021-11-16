using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Recruit.Server.Data;
using Recruit.Server.Services.BlobService;
using Recruit.Shared.Validators;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileValidator _fileValidator;
        private readonly IBlobService _blobService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IFileValidator fileValidator,
            IBlobService blobService,
            IWebHostEnvironment env, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _fileValidator = fileValidator;
            _blobService = blobService;
            _env = env;
            _configuration = configuration;
        }

        [HttpGet("MyProfile")]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user == null)
                return BadRequest();

            var profile = new ProfileViewModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                Avatar = user.Avatar,
                Headline = user.Headline
            };

            return Ok(profile);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (user == null)
                return BadRequest();

            user.FullName = viewModel.FullName;
            user.Headline = viewModel.Headline;
            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpPost("UpdateAvatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            if (!_fileValidator.IsValidPhoto(file))
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user == null)
                return BadRequest();

            // Upload the new avatar
            var blobName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var uploaded = await _blobService.UploadPhotoAsync(file, blobName);
            if (uploaded)
            {
                // Delete the old avatar
                var oldBlobName = Path.GetFileName(user.Avatar?.Split("?")[0]);
                if (!string.IsNullOrEmpty(oldBlobName))
                    await _blobService.DeletePhotoAsync(oldBlobName);

                // Update database
                string storageAccountName = _configuration["AzureBlobStorageSettings:StorageAccountName"];
                user.Avatar = _env.IsDevelopment() ?
                    $"http://127.0.0.1:10000/devstoreaccount1/photos/{blobName}" :
                    $"https://{storageAccountName}.blob.core.windows.net/photos/{blobName}";
                await _userManager.UpdateAsync(user);
            }

            return Ok(user.Avatar);
        }

        [HttpGet("RemoveAvatar")]
        public async Task<IActionResult> RemoveAvatar()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity?.Name);
            if (user == null)
                return BadRequest();

            // Delete from blob storage
            string? blobName = Path.GetFileName(user.Avatar?.Split("?")[0]);
            if (!string.IsNullOrEmpty(blobName))
                await _blobService.DeletePhotoAsync(blobName);

            // Remove avatar from user
            user.Avatar = null;
            await _userManager.UpdateAsync(user);

            return Ok();
        }
    }
}
