using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfiles.Api.Domain.Entities;

namespace UserProfiles.Api.Infrastructure.Persistence.EntityConfigurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasIndex(j => j.UserSub).IsUnique();
            //builder.Property(j => j.JobId).IsUnique();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            // column types tuned for PostgreSQL
            builder.Property(j => j.UserSub).IsRequired().HasColumnType("TEXT");
            builder.Property(j => j.JobId).IsRequired().HasColumnType("TEXT");
        }
    }
}
