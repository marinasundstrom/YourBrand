using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder
            .HasMany(p => p.Options)
            .WithMany(p => p.Products)
            .UsingEntity<ProductOption>();

        builder
            .HasMany(p => p.Attributes)
            .WithMany(p => p.Products)
            .UsingEntity<ProductAttribute>();

        builder
            .HasMany(x => x.Variants)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.ProductVariantOptions)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
