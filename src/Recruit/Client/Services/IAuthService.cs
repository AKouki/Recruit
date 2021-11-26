using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResult> Login(LoginViewModel user);
        Task<AuthResult> Register(RegisterViewModel user);
        Task<AuthResult> ChangePassword(ChangePasswordViewModel model);
        Task Logout();
    }
}
