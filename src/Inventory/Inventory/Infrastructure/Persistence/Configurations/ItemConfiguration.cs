using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Unit)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.GTIN)
            .HasMaxLength(64);

        builder.Property(i => i.Discontinued)
            .HasDefaultValue(false);

        builder.HasMany(i => i.WarehouseItems)
            .WithOne(w => w.Item)
            .HasForeignKey(w => w.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(i => i.WarehouseItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}