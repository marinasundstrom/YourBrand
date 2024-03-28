using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");
        //builder.HasQueryFilter(i => i.Deleted == null);

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