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

        builder.OwnsOne(x => x.Destination, destination =>
        {
            destination.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(128);

            destination.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(128);

            destination.Property(x => x.CareOf)
                .HasMaxLength(128);

            destination.OwnsOne(x => x.Address, address =>
            {
                address.Property(x => x.Street)
                    .IsRequired()
                    .HasMaxLength(256);

                address.Property(x => x.AddressLine2)
                    .HasMaxLength(256);

                address.Property(x => x.City)
                    .IsRequired()
                    .HasMaxLength(128);

                address.Property(x => x.StateOrProvince)
                    .HasMaxLength(128);

                address.Property(x => x.PostalCode)
                    .IsRequired()
                    .HasMaxLength(32);

                address.Property(x => x.Country)
                    .IsRequired()
                    .HasMaxLength(128);

                address.Property(x => x.CareOf)
                    .HasMaxLength(128);
            });
        });

        builder.Navigation(x => x.Destination).IsRequired();

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
