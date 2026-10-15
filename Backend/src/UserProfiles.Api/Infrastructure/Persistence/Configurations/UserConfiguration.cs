using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserProfiles.Api.Domain.Entities;

namespace UserProfiles.Api.Infrastructure.Persistence.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.IsVerified).HasDefaultValue(false);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            
            // column types tuned for PostgreSQL
            builder.Property(u => u.Email).IsRequired().HasColumnType("varchar(254)");
            builder.Property(u => u.FirstName).IsRequired().HasColumnType("varchar(100)");
            builder.Property(u => u.LastName).IsRequired().HasColumnType("varchar(100)");
            builder.Property(u => u.Sub).IsRequired().HasColumnType("text");
        }
    }
}
