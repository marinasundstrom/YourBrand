using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.ToTable("ProductAttributes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Product).WithMany(x => x.ProductAttributes)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductId })
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(o => o.Attribute).WithMany(x => x.ProductAttributes)
            .HasForeignKey(o => new { o.OrganizationId, o.AttributeId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}