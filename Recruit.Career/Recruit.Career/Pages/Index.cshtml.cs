using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recruit.Career.Models;

namespace Recruit.Career.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _clientFactory;


        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public IEnumerable<JobViewModel>? Jobs { get; set; }

        public async Task OnGet()
        {
            try
            {
                var client = _clientFactory.CreateClient("Recruit");
                Jobs = await client.GetFromJsonAsync<List<JobViewModel>>("api/JobOpenings");
            }
            catch (Exception)
            {
            }
        }
    }
}