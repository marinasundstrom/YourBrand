using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductOptionConfiguration : IEntityTypeConfiguration<ProductOption>
{
    public void Configure(EntityTypeBuilder<ProductOption> builder)
    {
        builder.ToTable("ProductOptions");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Product).WithMany(x => x.ProductOptions)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductId })
            .OnDelete(DeleteBehavior.ClientNoAction);

        builder.HasOne(o => o.Option).WithMany(x => x.ProductOption)
            .HasForeignKey(o => new { o.OrganizationId, o.OptionId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}