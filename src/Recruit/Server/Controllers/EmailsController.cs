using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Server.Models;
using Recruit.Server.Services.EmailService;
using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailsController(
            ApplicationDbContext db, 
            IEmailService emailService, 
            UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpGet("Applicant/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var applicant = await _db.Applicants.FindAsync(id);
            if (applicant == null)
                return NotFound();

            var emails = await _db.Emails
                .Include(e => e.Receiver)
                .Where(e => e.Receiver!.Id == applicant.Id)
                .Include(e => e.Sender)
                .Select(e => new EmailItemViewModel()
                {
                    Id = e.Id,
                    Body = e.Body,
                    SentDate = e.SentDate,
                    Sender = new UserViewModel()
                    {
                        FullName = e.Sender!.FullName,
                        Email = e.Sender!.Email,
                        Avatar = e.Sender!.Avatar
                    }
                }).ToListAsync();

            return Ok(emails);
        }

        [HttpPost("Send")]
        public async Task<IActionResult> Send(SendEmailViewModel model)
        {
            var applicant = await _db.Applicants
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.Id == model.ApplicantId);
            var sender = await _userManager.FindByEmailAsync(User.Identity!.Name);
            if (applicant == null || sender == null)
                return NotFound();

            try
            {
                // Send email
                var htmlMessage = GenerateHtmlMessage(model.Body, applicant, sender);
                await _emailService.SendEmailAsync(applicant.Email!, model.Subject!, htmlMessage);

                // Store email in database
                var sentEmail = new EmailItem()
                {
                    Sender = sender,
                    Receiver = applicant,
                    Subject = model.Subject?.Trim(),
                    Body = htmlMessage,
                    SentDate = DateTime.Now
                };

                _db.Emails.Add(sentEmail);
                await _db.SaveChangesAsync();

                return Ok(sentEmail);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var email = await _db.Emails.FindAsync(id);
            if (email == null)
                return NotFound();

            _db.Emails.Remove(email);
            await _db.SaveChangesAsync();

            return Ok();
        }

        private string GenerateHtmlMessage(string? body, Applicant applicant, ApplicationUser user)
        {
            body = body?.Replace("{candidate_name}", $"{applicant.FirstName} {applicant.LastName}");
            body = body?.Replace("{candidate_first_name}", applicant.FirstName);
            body = body?.Replace("{candidate_last_name}", applicant.LastName);
            body = body?.Replace("{job_title}", applicant.Job?.Title);
            body = body?.Replace("{user}", user.FullName);
            body = body?.Replace("\n", "<br/>");
            return body?.Trim() ?? "";
        }
    }
}
