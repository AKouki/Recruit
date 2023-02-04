using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly ApplicationDbContext _db;

        public SearchController(ILogger<SearchController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<SearchResult> Get(string searchTerm)
        {
            var searchResult = new SearchResult();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                var jobs = await _db.Jobs
                    .AsNoTracking()
                    .Where(j => j.Title!.Contains(searchTerm))
                    .ToListAsync();

                var applicants = await _db.Applicants
                    .AsNoTracking()
                    .Where(a => a.FirstName!.Contains(searchTerm) || a.LastName!.Contains(searchTerm))
                    .Include(a => a.Job)
                    .ToListAsync();

                searchResult.Jobs = jobs;
                searchResult.Applicants = applicants;
            }

            return searchResult;
        }
    }
}
