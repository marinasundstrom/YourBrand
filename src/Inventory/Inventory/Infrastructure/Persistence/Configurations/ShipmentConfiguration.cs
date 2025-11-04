using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNo)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Destination)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Service)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Shipment)
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
