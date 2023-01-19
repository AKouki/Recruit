using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Recruit.Server.Services.EmailService
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridOptions _sendGridOptions;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(IOptions<SendGridOptions> options, ILogger<SendGridEmailService> logger)
        {
            _sendGridOptions = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlContent)
        {
            //var client = new SendGridClient(_sendGridOptions.ApiKey);
            //var from = new EmailAddress(_sendGridOptions.FromEmail, _sendGridOptions.FromName);
            //var to = new EmailAddress(email);
            //var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent, htmlContent);
            //var response = await client.SendEmailAsync(msg);
            //if (!response.IsSuccessStatusCode)
            //{
            //    _logger.LogError($"Send email to {email} failed!");
            //}

            await Task.Delay(0);
        }
    }
}
