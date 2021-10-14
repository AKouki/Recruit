using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicantsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ApplicantsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IEnumerable<Applicant>>? Get()
        {
            var applicants = await _db.Applicants
                .Include(a => a.Job!)
                    .ThenInclude(s => s.Stages)
                .Include(a => a.Interview)
                .Include(a => a.Education)
                .Include(a => a.Experience)
                .Include(a => a.Resume)
                .OrderByDescending(a => a.ApplyDate)
                .ToListAsync();

            return applicants;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var applicant = await _db.Applicants
                .Include(a => a.Job!)
                    .ThenInclude(s => s.Stages)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicant == null)
                return NotFound();

            return Ok(applicant);
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusModel model)
        {
            var applicant = await _db.Applicants.FirstOrDefaultAsync(a => a.Id == model.ApplicantId);
            var newStage = await _db.Stages.FirstOrDefaultAsync(s => s.Id == model.StageId);

            if (applicant == null || newStage == null)
                return BadRequest();

            applicant.Stage = newStage;

            _db.Applicants.Update(applicant);
            await _db.SaveChangesAsync();

            return Ok(applicant);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var applicant = _db.Applicants.Find(id);
            if (applicant == null)
                return NotFound();

            _db.Applicants.Remove(applicant);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
