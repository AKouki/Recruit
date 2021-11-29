using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Shared;

namespace Recruit.Server.Data.Configuration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasIndex(d => d.Name).IsUnique();

            builder.HasMany(d => d.Jobs)
                .WithOne(j => j.Department)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
