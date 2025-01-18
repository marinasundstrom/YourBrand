
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class EmploymentRoleConfiguration : IEntityTypeConfiguration<EmploymentRole>
{
    public void Configure(EntityTypeBuilder<EmploymentRole> builder)
    {
        builder.ToTable("EmploymentRoles");

        builder.HasIndex(x => x.TenantId);

        builder
            .HasOne(p => p.Employment)
            .WithMany(x => x.Roles)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(p => p.Assignment)
            .WithMany(x => x.Roles)
            .OnDelete(DeleteBehavior.NoAction);
    }
}