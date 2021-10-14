using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class InviteModel
    {
        [Required]
        public int JobId { get; set; }
        [Required]
        [EmailAddress]
        public string? ApplicantEmail { get; set; }
    }
}
