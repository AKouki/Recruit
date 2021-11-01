using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Shared;

namespace Recruit.Server.Data.Configuration
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.ToTable("Jobs");

            builder.HasMany(j => j.Stages)
                .WithOne(s => s.Job)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(j => j.Applicants)
                .WithOne(s => s.Job)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
