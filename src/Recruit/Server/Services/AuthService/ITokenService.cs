namespace Recruit.Server.Services.AuthService
{
    public interface ITokenService
    {
        string GenerateToken(string? email, string? fullName);
    }
}
