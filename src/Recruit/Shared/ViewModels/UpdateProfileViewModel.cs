using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(100, ErrorMessage = "The {0} can be up to {1} characters long.")]
        public string? Headline { get; set; }
    }
}
