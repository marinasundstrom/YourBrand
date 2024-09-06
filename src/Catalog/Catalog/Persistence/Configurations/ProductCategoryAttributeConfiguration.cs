using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductGroupAttributeConfiguration : IEntityTypeConfiguration<ProductCategoryAttribute>
{
    public void Configure(EntityTypeBuilder<ProductCategoryAttribute> builder)
    {
        builder.ToTable("ProductCategoryAttributes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.ProductCategory).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.ProductCategoryId })
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(o => o.Attribute).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.AttributeId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}