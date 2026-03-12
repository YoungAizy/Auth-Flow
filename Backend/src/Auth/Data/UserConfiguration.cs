using Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Verified).HasDefaultValue(false);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            
            // column types tuned for PostgreSQL
            builder.Property(u => u.Email).IsRequired().HasColumnType("varchar(254)");
            builder.Property(u => u.FirstName).IsRequired().HasColumnType("varchar(100)");
            builder.Property(u => u.LastName).IsRequired().HasColumnType("varchar(100)");
            //builder.Property(u => u.Password).IsRequired().HasColumnType("text");
        }
    }
}
