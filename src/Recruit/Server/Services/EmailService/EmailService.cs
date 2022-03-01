using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Recruit.Server.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailSettings> emailSettings, 
            ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            //try
            //{
            //    var client = new SmtpClient()
            //    {
            //        Host = _emailSettings.Host!,
            //        Port = _emailSettings.Port,
            //        UseDefaultCredentials = true,
            //        EnableSsl = _emailSettings.EnableSSL,
            //        Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
            //    };

            //    var emailMessage = new MailMessage()
            //    {
            //        From = new MailAddress(_emailSettings.Username!),
            //        Subject = subject,
            //        Body = htmlMessage,
            //        IsBodyHtml = true
            //    };
            //    emailMessage.To.Add(new MailAddress(email));

            //    return client.SendMailAsync(emailMessage);
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError($"Failed to send email to: {email} with subject: {subject}.", ex.Message);
            //    throw;
            //}

            return Task.CompletedTask;
        }
    }
}
