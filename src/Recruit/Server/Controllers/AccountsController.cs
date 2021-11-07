using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recruit.Server.Services.AuthService;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AccountsController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserViewModel model)
        {
            var result = await _authService.LoginAsync(model.Email, model.Password);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel model)
        {
            var result = await _authService.RegisterAsync(model.Email, model.Password, model.FullName);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            var result = await _authService.ChangePasswordAsync(User.Identity?.Name, viewModel.OldPassword, viewModel.NewPassword);
            if (result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

    }
}
