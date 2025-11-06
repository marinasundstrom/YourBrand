using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductPriceTierConfiguration : IEntityTypeConfiguration<ProductPriceTier>
{
    public void Configure(EntityTypeBuilder<ProductPriceTier> builder)
    {
        builder.ToTable("ProductPriceTiers");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.Property(x => x.FromQuantity)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.TierType)
            .HasConversion<int>();

        builder.HasOne(x => x.ProductPrice)
            .WithMany(x => x.PriceTiers)
            .HasForeignKey(x => new { x.OrganizationId, x.ProductPriceId });
    }
}
