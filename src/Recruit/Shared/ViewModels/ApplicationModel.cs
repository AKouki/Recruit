using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class ApplicationModel
    {
        public int JobId { get; set; }

        [Required]
        [Display(Name = "First name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string? LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Phone")]
        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Headline { get; set; }

        public string? Skills { get; set; }

        public string? Summary { get; set; }

        public IFormFile? Photo { get; set; }

        //public List<Education>? Education { get; set; }
        //public List<Experience>? Experience { get; set; }
        public string? EducationJson { get; set; }
        public string? ExperienceJson { get; set; }

        [Required]
        public IFormFile? Resume { get; set; }
    }
}
