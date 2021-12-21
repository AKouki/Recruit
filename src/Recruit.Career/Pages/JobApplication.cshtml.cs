using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Recruit.Career.Models;
using Recruit.Shared.Validators;
using System.Text;
using System.Text.Json;

namespace Recruit.Career.Pages
{
    public class JobApplicationModel : PageModel
    {
        private readonly ILogger<JobApplicationModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFileValidator _fileValidator;
        public JobApplicationModel(ILogger<JobApplicationModel> logger, 
            IHttpClientFactory httpClientFactory,
            IFileValidator fileValidator)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _fileValidator = fileValidator;
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
                var client = _httpClientFactory.CreateClient("Recruit");
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

            if (!_fileValidator.IsValidResume(Application.Resume))
            {
                ModelState.AddModelError("InvalidResumeFile", "Invalid resume file. Allowed file size: 2MB / Allowed file extensions: docx, pdf");
                return Page();
            }

            if (Application.Photo != null && !_fileValidator.IsValidPhoto(Application.Photo))
            {
                ModelState.AddModelError("InvalidPhotoFile", "Invalid photo file. Allowed file size: 1MB / Allowed file extensions: jpg, jpeg");
                return Page();
            }

            try
            {
                var client = _httpClientFactory.CreateClient("Recruit");

                var content = new MultipartFormDataContent();
                AddProfileInformation(content);
                AddEducationAndExperience(content);
                AddPhotoAndResume(content);

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
            catch (Exception ex)
            {
                _logger.LogError($"Error submiting {Application.Email} job application for position {Application.JobId}.", ex.Message);
            }

            return Page();
        }

        private void AddProfileInformation(MultipartFormDataContent content)
        {
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
        }

        private void AddEducationAndExperience(MultipartFormDataContent content)
        {
            if (Application.Education.Any())
            {
                var json = JsonSerializer.Serialize(Application.Education.Take(5));
                content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "EducationJson");
            }

            if (Application.Experience.Any())
            {
                var json = JsonSerializer.Serialize(Application.Experience.Take(5));
                content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "ExperienceJson");
            }
        }

        private void AddPhotoAndResume(MultipartFormDataContent content)
        {
            if (Application?.Photo != null)
                content.Add(new StreamContent(Application.Photo.OpenReadStream()), "Photo", Application.Photo.FileName);

            if (Application?.Resume != null)
                content.Add(new StreamContent(Application.Resume.OpenReadStream()), "Resume", Application.Resume.FileName);
        }
    }
}
