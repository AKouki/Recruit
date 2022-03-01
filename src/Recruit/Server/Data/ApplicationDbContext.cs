using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Recruit.Server.Models;
using Recruit.Shared;
using System.Reflection;

namespace Recruit.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<Applicant> Applicants => Set<Applicant>();
        public DbSet<Stage> Stages => Set<Stage>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Template> Templates => Set<Template>();
        public DbSet<EmailItem> Emails => Set<EmailItem>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
