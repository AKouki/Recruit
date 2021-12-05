using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class InviteViewModel
    {
        [Required]
        public int JobId { get; set; }

        public List<string>? Emails { get; set; }
    }
}
