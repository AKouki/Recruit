using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Recruit.Server.Data;
using Recruit.Shared;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public EmailTemplatesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<EmailTemplate> Get()
        {
            return _db.EmailTemplates
                .Select(t => new EmailTemplate { Id = t.Id, Name = t.Name })
                .ToList();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var emailTemplate = await _db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
                return NotFound();

            return Ok(emailTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmailTemplate model)
        {
            var newEmailTemplate = new EmailTemplate()
            {
                Name = model.Name,
                Subject = model.Subject,
                Body = model.Body
            };

            _db.EmailTemplates.Add(newEmailTemplate);
            await _db.SaveChangesAsync();

            return Ok(newEmailTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EmailTemplate model)
        {
            var emailTemplate = await _db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
                return NotFound();

            emailTemplate.Name = model.Name;
            emailTemplate.Subject = model.Subject;
            emailTemplate.Body = model.Body;

            _db.EmailTemplates.Update(emailTemplate);
            await _db.SaveChangesAsync();

            return Ok(emailTemplate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var emailTemplate = await _db.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
                return NotFound();

            _db.EmailTemplates.Remove(emailTemplate);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
