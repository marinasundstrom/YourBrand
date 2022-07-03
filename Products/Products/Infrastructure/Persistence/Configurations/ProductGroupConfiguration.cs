using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Infrastructure.Persistence.Configurations;

public class ProductGroupConfiguration : IEntityTypeConfiguration<ProductGroup>
{
    public void Configure(EntityTypeBuilder<ProductGroup> builder)
    {
        builder.ToTable("ProductGroups");
        //builder.HasQueryFilter(i => i.Deleted == null);
    }
}
