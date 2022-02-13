using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared;
using Recruit.Shared.Enums;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public TemplatesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("EmailTemplates")]
        public async Task<IEnumerable<Template>> GetEmailTemplates()
        {
            var templates = await _db.Templates
                .Where(t => t.TemplateType == TemplateType.Email)
                .ToListAsync();

            return templates;
        }

        [HttpGet("JobTemplates")]
        public async Task<IEnumerable<Template>> GetJobTemplates()
        {
            // Performance tuning: Do not include job description as they are too long.
            // The job description can be loaded by making a 2nd request for a specific template.
            var templates = await _db.Templates
                .Where(t => t.TemplateType == TemplateType.Job)
                .Select(t => new Template()
                {
                    Id = t.Id,
                    Name = t.Name,
                    TemplateType = t.TemplateType
                })
                .ToListAsync();

            return templates;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Template model)
        {
            var newTemplate = new Template()
            {
                Name = model.Name,
                Body = model.Body,
                TemplateType = model.TemplateType,
            };

            _db.Templates.Add(newTemplate);
            await _db.SaveChangesAsync();

            return Ok(newTemplate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Template model)
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            template.Name = model.Name;
            template.Body = model.Body;
            template.TemplateType = model.TemplateType;

            _db.Templates.Update(template);
            await _db.SaveChangesAsync();

            return Ok(template);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await _db.Templates.FindAsync(id);
            if (template == null)
                return NotFound();

            _db.Templates.Remove(template);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
