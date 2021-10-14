using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Shared;

namespace Recruit.Server.Data.Configuration
{
    public class ApplicantConfiguration : IEntityTypeConfiguration<Applicant>
    {
        public void Configure(EntityTypeBuilder<Applicant> builder)
        {
            builder.ToTable("Applicants");

            builder.HasIndex(a => new {a.Email, a.JobId}).IsUnique();

            builder.HasOne(a => a.Stage)
                .WithMany(s => s.Applicants)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Interview)
                .WithOne(i => i.Applicant)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Education)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(a => a.Experience)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Resume)
                .WithOne(x => x.Applicant)
                .HasForeignKey<Attachment>(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
