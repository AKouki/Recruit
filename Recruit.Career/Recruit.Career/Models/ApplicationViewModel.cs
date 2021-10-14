using System.ComponentModel.DataAnnotations;

namespace Recruit.Career.Models
{
    public class ApplicationViewModel
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
        [MinLength(10, ErrorMessage = "The {0} must be at least {1}-digit number.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Headline { get; set; }

        public string? Skills { get; set; }

        public string? Summary { get; set; }

        public IFormFile? Photo { get; set; }

        // Education
        // Experience

        [Required]
        public IFormFile? Resume { get; set; }
    }
}
