using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<UserViewModel> Get()
        {
            var users = _db.Users.Select(u => new UserViewModel()
            {
                FullName = u.FullName,
                Email = u.Email,
                Username = u.UserName,
                Avatar = u.Avatar
            }).ToList();

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

            try
            {
                await _userManager.DeleteAsync(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
