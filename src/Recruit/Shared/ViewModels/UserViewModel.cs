using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class UserViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
