using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductPriceConfiguration : IEntityTypeConfiguration<ProductPrice>
{
    public void Configure(EntityTypeBuilder<ProductPrice> builder)
    {
        builder.ToTable("ProductPrices");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(x => x.Region)
            .WithMany()
            .HasForeignKey(x => x.RegionId);
        
        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryCode);

        builder.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyCode);
    }
}