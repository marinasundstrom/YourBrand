using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductVariantOptionConfiguration : IEntityTypeConfiguration<ProductVariantOption>
{
    public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
    {
        builder.ToTable("ProductVariantOption");

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.ProductVariantOptions)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.ProductVariantOptions)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Option)
            .WithMany(x => x.ProductVariantOptions)
            .OnDelete(DeleteBehavior.NoAction);
    }
}