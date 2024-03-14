using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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