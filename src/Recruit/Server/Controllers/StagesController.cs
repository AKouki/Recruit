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
    public class StagesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public StagesController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<IEnumerable<Stage>> Get(int id)
        {
            var stages = await _db.Stages
                .Include(s => s.Job)
                .Where(s => s.Job!.Id == id)
                .ToListAsync();

            return stages.OrderBy(s => s.Id);
        }
    }
}
