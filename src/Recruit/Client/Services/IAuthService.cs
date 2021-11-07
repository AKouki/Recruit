using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Client.Services
{
    public interface IAuthService
    {
        Task<AuthResult> Login(UserViewModel user);
        Task<AuthResult> Register(UserViewModel user);
        Task<AuthResult> ChangePassword(ChangePasswordViewModel model);
        Task Logout();
    }
}
