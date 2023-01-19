namespace Recruit.Server.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlContent);
    }
}
