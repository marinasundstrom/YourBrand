using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class SupplierOrderConfiguration : IEntityTypeConfiguration<SupplierOrder>
{
    public void Configure(EntityTypeBuilder<SupplierOrder> builder)
    {
        builder.ToTable("SupplierOrders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SupplierId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.OrderNumber)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.OrderedAt)
            .IsRequired();

        builder.Property(x => x.ExpectedDelivery);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
