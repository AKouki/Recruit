using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Data;
using Recruit.Shared;

namespace Recruit.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DepartmentsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Department> Get()
        {
            return _db.Departments.OrderBy(d => d.Id).ToList();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Department model)
        {
            var exists = _db.Departments.Any(d => d.Name == model.Name);
            if (exists)
                return BadRequest();

            var newDepartment = new Department()
            {
                Name = model.Name
            };

            _db.Departments.Add(newDepartment);
            await _db.SaveChangesAsync();

            return Ok(newDepartment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Department model)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            department.Name = model.Name;

            _db.Departments.Update(department);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _db.Departments.FindAsync(id);
            if (department == null)
                return NotFound();

            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();

            return Ok();
        }
    }
}
