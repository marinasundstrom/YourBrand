using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Domain.ValueObjects;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
{
    public void Configure(EntityTypeBuilder<WarehouseItem> builder)
    {
        builder.ToTable("WarehouseItems");

        builder.Property(w => w.Location)
            .HasConversion(
                location => location.Label,
                label => StorageLocation.Create(label))
            .HasColumnName("Location")
            .HasMaxLength(128);

        builder.Ignore(w => w.IsBelowThreshold);

        builder.HasOne(w => w.Item)
            .WithMany(i => i.WarehouseItems)
            .HasForeignKey(w => w.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(w => w.Warehouse)
            .WithMany(warehouse => warehouse.Items)
            .HasForeignKey(w => w.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
