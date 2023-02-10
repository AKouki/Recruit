using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared.ViewModels;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MembersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get()
        {
            var users = await _db.Users.Select(u => new UserViewModel()
            {
                FullName = u.FullName,
                Email = u.Email,
                Username = u.UserName,
                Avatar = u.Avatar
            }).ToListAsync();

            return users;
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> Delete(string userName)
        {
            // Can't delete your own account
            if (userName == User.Identity?.Name)
                return BadRequest();

            // If theres only one account, you can't delete it
            var totalAccounts = _db.Users.Count();
            if (totalAccounts < 2)
                return BadRequest();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}
