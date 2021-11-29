using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Recruit.Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Headline { get; set; }
    }
}
