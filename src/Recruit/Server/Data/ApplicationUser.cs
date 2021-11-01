using Microsoft.AspNetCore.Identity;

namespace Recruit.Server.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
