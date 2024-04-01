using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        //in case you chagned the TKey type
        //  builder.HasKey(key => new { key.UserId, key.RoleId });

        builder.Property(e => e.UserId).HasColumnName("UserId");
    }
}