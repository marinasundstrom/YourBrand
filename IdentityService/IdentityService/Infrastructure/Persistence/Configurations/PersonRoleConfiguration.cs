using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<PersonRole>
{
    public void Configure(EntityTypeBuilder<PersonRole> builder)
    {
        builder.ToTable("PersonRoles");

        //in case you chagned the TKey type
        //  builder.HasKey(key => new { key.UserId, key.RoleId });

        builder.Property(e => e.UserId).HasColumnName("PersonId");
    }
}
