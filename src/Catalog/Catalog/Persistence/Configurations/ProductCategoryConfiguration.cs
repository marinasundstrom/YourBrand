using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        //builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Store).WithMany(x => x.Categories)
         .HasForeignKey(o => new { o.OrganizationId, o.StoreId });

        builder.HasOne(o => o.Parent).WithMany(x => x.SubCategories)
      .HasForeignKey(o => new { o.OrganizationId, o.ParentId });

        builder.HasIndex(x => x.TenantId);

        builder
            .Property(x => x.Handle)
            .HasMaxLength(150);

        builder
            .Property(x => x.Path)
            .HasMaxLength(150);

        builder.HasIndex(x => x.Handle);
        builder.HasIndex(x => x.Path);
    }
}