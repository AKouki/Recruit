using Recruit.Shared;

namespace Recruit.Server.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RegisterAsync(string email, string password, string fullName);
        Task<AuthResult> ChangePasswordAsync(string email, string oldPassword, string newPassword);
    }
}
