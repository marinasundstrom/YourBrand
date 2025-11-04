using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class SupplierItemConfiguration : IEntityTypeConfiguration<SupplierItem>
{
    public void Configure(EntityTypeBuilder<SupplierItem> builder)
    {
        builder.ToTable("SupplierItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasMaxLength(64);

        builder.Property(x => x.SupplierId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ItemId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.SupplierItemId)
            .HasMaxLength(128);

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Item)
            .WithMany(x => x.SupplierItems)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.OrderLines)
            .WithOne(x => x.SupplierItem)
            .HasForeignKey(x => x.SupplierItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
