namespace Recruit.Server.Services
{
    public interface ITokenService
    {
        string GenerateToken(string? email, string? fullName);
    }
}
