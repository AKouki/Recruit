using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recruit.Career.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Recruit.Career.Pages
{
    public class JobApplicationModel : PageModel
    {
        private readonly ILogger<JobApplicationModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public JobApplicationModel(ILogger<JobApplicationModel> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // static so we can keep state of job title
        public static JobViewModel? Job { get; set; }

        [BindProperty]
        public ApplicationViewModel Application { get; set; } = new();

        public async Task OnGet()
        {
            try
            {
                var client = _clientFactory.CreateClient("Recruit");
                Job = await client.GetFromJsonAsync<JobViewModel>("api/JobOpenings/" + Id);
            }
            catch (Exception)
            {
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!ValidateResume(Application.Resume))
            {
                ModelState.AddModelError("InvalidResumeFile", "Invalid resume file. Allowed file size: 2MB / Allowed file extensions: docx, pdf");
                return Page();
            }

            if (Application.Photo != null && !ValidatePhoto(Application.Photo))
            {
                ModelState.AddModelError("InvalidPhotoFile", "Invalid photo file. Allowed file size: 1MB / Allowed file extensions: jpg, jpeg");
                return Page();
            }

            try
            {
                var client = _clientFactory.CreateClient("Recruit");

                var content = new MultipartFormDataContent();
                content.Add(new StringContent(Application.JobId.ToString()), "JobId");
                content.Add(new StringContent(Application.FirstName!), "FirstName");
                content.Add(new StringContent(Application.LastName!), "LastName");
                content.Add(new StringContent(Application.Email!), "Email");
                content.Add(new StringContent(Application.Phone!), "Phone");
                if (Application.Address != null)
                    content.Add(new StringContent(Application.Address), "Address");
                if (Application.Headline != null)
                    content.Add(new StringContent(Application.Headline), "Headline");
                if (Application.Skills != null)
                    content.Add(new StringContent(Application.Skills), "Skills");
                if (Application.Summary != null)
                    content.Add(new StringContent(Application.Summary), "Summary");

                // Photo & Resume
                if (Application?.Photo != null)
                    content.Add(new StreamContent(Application.Photo.OpenReadStream()), "Photo", Application.Photo.FileName);

                if (Application?.Resume != null)
                    content.Add(new StreamContent(Application.Resume.OpenReadStream()), "Resume", Application.Resume.FileName);

                var response = await client.PostAsync("api/JobOpenings/Apply", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("Success");
                }
                else
                {
                    ModelState.AddModelError("InvalidSubmission", "Invalid submission. Please make sure you have not applied again for this job with this email.");
                    return Page();
                }
            }
            catch (Exception)
            {
            }

            return Page();
        }

        private bool ValidateFile(IFormFile? file, int maxAllowedFileSize, string allowedExtensionsStr)
        {
            if (file != null)
            {
                string[] AllowedExtenstions = allowedExtensionsStr.Split(',');

                if (file.Length > maxAllowedFileSize)
                    return false;

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !AllowedExtenstions.Any(s => s.Contains(extension)))
                    return false;

                if (file.FileName.Length > 255)
                    return false;

                return true;
            }

            return false;
        }

        private bool ValidateResume(IFormFile? file)
        {
            return ValidateFile(file, 2 * 1024 * 1024, ".docx,.pdf");
        }

        private bool ValidatePhoto(IFormFile? file)
        {
            return ValidateFile(file, 1 * 1024 * 1024, ".jpg,.jpeg");
        }
    }
}
