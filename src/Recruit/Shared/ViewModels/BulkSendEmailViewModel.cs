using Recruit.Shared.Validators;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class BulkSendEmailViewModel
    {
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Body { get; set; }
        [Required]
        [MinElements(1, ErrorMessage = "The {0} must have atleast {1} item.")]
        public List<int>? ApplicantIds { get; set; } = new();
    }
}
