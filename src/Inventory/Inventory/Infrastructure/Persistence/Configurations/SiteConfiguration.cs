using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class SiteConfiguration : IEntityTypeConfiguration<Site>
{
    public void Configure(EntityTypeBuilder<Site> builder)
    {
        builder.ToTable("Sites");

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.IsActive)
            .HasDefaultValue(true);

        builder.Navigation(s => s.Warehouses)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
