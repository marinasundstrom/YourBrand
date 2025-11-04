using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.IsActive)
            .HasDefaultValue(true);

        builder.Navigation(w => w.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasOne(w => w.Site)
            .WithMany(s => s.Warehouses)
            .HasForeignKey(w => w.SiteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
