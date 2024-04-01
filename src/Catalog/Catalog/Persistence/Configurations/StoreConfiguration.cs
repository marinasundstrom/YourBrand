using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey("CurrencyCode");

        builder.OwnsOne(x => x.CurrencyDisplayOptions);

        builder.OwnsOne(x => x.PricingOptions, x => x.OwnsMany(z => z.CategoryPricingOptions));
    }
}