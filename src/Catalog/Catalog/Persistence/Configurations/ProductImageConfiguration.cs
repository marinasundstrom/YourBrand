using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder
           .HasOne(x => x.Product)
           .WithMany(x => x.Images)
           .HasForeignKey(x => new { x.OrganizationId, x.ProductId })
           .OnDelete(DeleteBehavior.Cascade);
    }
}