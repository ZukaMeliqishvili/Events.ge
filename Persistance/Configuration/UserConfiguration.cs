using Domain.User;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Persistance.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(i => i.UserName).IsUnicode(false);
            builder.Property(i => i.Email).IsUnicode(false);
            builder.HasIndex(i => i.Email).IsUnique(true);
            builder.HasIndex(i => i.UserName).IsUnique(true);
        }
    }
}
