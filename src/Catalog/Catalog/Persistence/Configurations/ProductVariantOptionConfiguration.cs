using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductVariantOptionConfiguration : IEntityTypeConfiguration<ProductVariantOption>
{
    public void Configure(EntityTypeBuilder<ProductVariantOption> builder)
    {
        builder.ToTable("ProductVariantOption");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        /*
             builder.HasOne(o => o.Product).WithMany(x => x.ProductVariantOptions)
                 .HasForeignKey(o => new { o.OrganizationId, o.ProductId })
                 .OnDelete(DeleteBehavior.ClientNoAction); */

        builder.HasOne(o => o.ProductVariant).WithMany(x => x.ProductVariantOptions)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductVariantId })
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(o => o.Option).WithMany(x => x.ProductVariantOptions)
            .HasForeignKey(o => new { o.OrganizationId, o.OptionId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}