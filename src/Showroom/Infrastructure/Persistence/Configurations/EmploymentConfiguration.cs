
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.ToTable("Employments");

        builder.HasIndex(x => x.TenantId);

        builder
            .HasMany(p => p.Roles)
            .WithOne(x => x.Employment)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Assignments)
            .WithOne(x => x.Employment)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(p => p.Skills)
            .WithOne(x => x.Employment)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
