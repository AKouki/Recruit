using Microsoft.AspNetCore.Identity;
using Recruit.Server.Data;
using Recruit.Shared;

namespace Recruit.Server.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResult()
                {
                    Errors = new List<string>() { "Invalid email or password" }
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (isValidPassword == true)
            {
                return new AuthResult()
                {
                    Succeeded = true,
                    Token = _tokenService.GenerateToken(email, user.FullName!),
                    FullName = user.FullName,
                    Avatar = user.Avatar
                };
            }

            return new AuthResult()
            {
                Errors = new List<string>() { "Invalid email or password" }
            };
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string fullName)
        {
            var user = new ApplicationUser() { UserName = email, Email = email, FullName = fullName };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return new AuthResult()
                {
                    Succeeded = true,
                    Token = _tokenService.GenerateToken(email, fullName),
                    FullName = user.FullName,
                    Avatar = user.Avatar
                };
            }

            return new AuthResult()
            {
                Errors = result.Errors.Select(x => x.Description)
            };
        }

        public async Task<AuthResult> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthResult("Invalid password");

            // Validate password format
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var result = await passwordValidator.ValidateAsync(_userManager, user, newPassword);
            if (!result.Succeeded)
            {
                return new AuthResult()
                {
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Validate password and change it
            var identityResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            if (identityResult.Succeeded)
            {
                return new AuthResult()
                {
                    Succeeded = true,
                    Token = _tokenService.GenerateToken(email, user.FullName!),
                    FullName = user.FullName,
                    Avatar = user.Avatar
                };
            }

            return new AuthResult("Invalid password");
        }

    }
}
