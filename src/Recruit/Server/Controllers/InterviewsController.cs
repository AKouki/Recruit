using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public InterviewsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Interview>? Get()
        {
            var interviews =_db.Interviews
                .Include(s => s.Applicant!)
                    .ThenInclude(a => a.Job)
                .OrderBy(i => i.ScheduledAt)
                .ToList();

            return interviews;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScheduleInterviewModel model)
        {
            var applicant = _db.Applicants.Find(model.ApplicantId);
            if (applicant == null)
            {
                return NotFound();
            }

            if (Exists(applicant))
            {
                return BadRequest("Another interview exists for this Applicant!");
            }

            var manager = _db.Users.FirstOrDefault(u => u.UserName == User.Identity!.Name);
            var interview = new Interview()
            {
                Applicant = applicant,
                ScheduledAt = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, model.Time.Hour, model.Time.Minute, 0),
                Duration = model.Duration,
                Interviewer = manager == null ? "Emma Stone" : manager.FullName
            };

            _db.Interviews.Add(interview);
            await _db.SaveChangesAsync();

            return Ok(interview);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ScheduleInterviewModel model)
        {
            var interviews = _db.Interviews.ToList();
            var interview = _db.Interviews.FirstOrDefault(x => x.Id == id);

            Console.WriteLine("Interview id: " + id);

            if (interview == null)
            {
                return NotFound();
            }

            interview.ScheduledAt = new DateTime(model.Date.Year, model.Date.Month, model.Date.Day, model.Time.Hour, model.Time.Minute, 0);
            interview.Duration = model.Duration;

            _db.Interviews.Update(interview);
            await _db.SaveChangesAsync();

            return Ok(interview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var interview = _db.Interviews.Find(id);
            if (interview == null)
                return NotFound();

            _db.Interviews.Remove(interview);
            await _db.SaveChangesAsync();

            return Ok();
        }

        private bool Exists(Applicant applicant)
        {
            var interview = _db.Interviews.Include(i => i.Applicant)
                .Where(i => i.Applicant!.Id == applicant!.Id)
                .FirstOrDefault();

            return interview != null;
        }
    }
}
