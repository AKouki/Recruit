using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recruit.Shared;

namespace Recruit.Server.Data.Configuration
{
    public class InterviewConfiguration : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> builder)
        {
            builder.ToTable("Interviews");

            builder.HasIndex(i => i.ApplicantId).IsUnique();
        }
    }
}
