using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.Server.Services.BlobService;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttachmentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IBlobService _blobService;

        public AttachmentsController(IConfiguration configuration,
            IWebHostEnvironment env,
            IBlobService blobService)
        {
            _configuration = configuration;
            _env = env;
            _blobService = blobService;
        }

        [HttpGet("Resume/{fileName}")]
        public async Task<IActionResult> ViewResume(string fileName)
        {
            try
            {
                byte[] fileBytes = await _blobService.GetAsync(fileName);
                return File(fileBytes, "application/pdf");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
