using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;
namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class WarehouseItemReservationConfiguration : IEntityTypeConfiguration<WarehouseItemReservation>
{
    public void Configure(EntityTypeBuilder<WarehouseItemReservation> builder)
    {
        builder.ToTable("WarehouseItemReservations");

        builder.Property(r => r.Quantity)
            .IsRequired();

        builder.Property(r => r.ConsumedQuantity)
            .IsRequired();

        builder.Property(r => r.ReleasedQuantity)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(r => r.ReservedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.Reference)
            .HasMaxLength(256);

        builder.HasOne(r => r.WarehouseItem)
            .WithMany(w => w.Reservations)
            .HasForeignKey(r => r.WarehouseItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
