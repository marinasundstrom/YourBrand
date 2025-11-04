using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class SupplierOrderLineConfiguration : IEntityTypeConfiguration<SupplierOrderLine>
{
    public void Configure(EntityTypeBuilder<SupplierOrderLine> builder)
    {
        builder.ToTable("SupplierOrderLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.SupplierItemId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.QuantityOrdered)
            .IsRequired();

        builder.Property(x => x.ExpectedQuantity)
            .IsRequired();

        builder.Property(x => x.QuantityReceived)
            .IsRequired();
    }
}
