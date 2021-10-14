using Recruit.Shared;

namespace Recruit.Server.Services
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string? email, string? password);
        Task<AuthResult> RegisterAsync(string? email, string? password, string? fullName);
    }
}
