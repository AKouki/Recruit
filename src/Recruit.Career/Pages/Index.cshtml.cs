using Microsoft.AspNetCore.Mvc.RazorPages;
using Recruit.Career.Models;

namespace Recruit.Career.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IEnumerable<JobViewModel>? Jobs { get; set; }

        public async Task OnGet()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Recruit");
                Jobs = await client.GetFromJsonAsync<List<JobViewModel>>("api/JobOpenings");
            }
            catch (Exception)
            {
            }
        }
    }
}