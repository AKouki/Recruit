using Recruit.Shared.Validators;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Shared.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [NotEqual("OldPassword", ErrorMessage = "The {0} and {1} must not be the same.")]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? NewPasswordConfirm { get; set; }
    }
}
