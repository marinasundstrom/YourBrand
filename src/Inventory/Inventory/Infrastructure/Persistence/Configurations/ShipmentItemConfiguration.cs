using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class ShipmentItemConfiguration : IEntityTypeConfiguration<ShipmentItem>
{
    public void Configure(EntityTypeBuilder<ShipmentItem> builder)
    {
        builder.ToTable("ShipmentItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.HasIndex(x => new { x.ShipmentId, x.WarehouseItemId })
            .IsUnique();

        builder.HasOne(x => x.Shipment)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WarehouseItem)
            .WithMany()
            .HasForeignKey(x => x.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
