using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recruit.Career.Models;

namespace Recruit.Career.Pages
{
    public class JobDetailsModel : PageModel
    {
        private readonly ILogger<JobDetailsModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public JobDetailsModel(ILogger<JobDetailsModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public JobViewModel? Job { get; set; }

        public async Task OnGet()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Recruit");
                Job = await client.GetFromJsonAsync<JobViewModel>("api/JobOpenings/" + Id);
            }
            catch (Exception)
            {
            }
        }
    }
}
