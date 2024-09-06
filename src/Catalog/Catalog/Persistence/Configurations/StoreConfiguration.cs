using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable("Stores");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey("CurrencyCode");

        builder.OwnsOne(x => x.CurrencyDisplayOptions);

        builder.OwnsOne(x => x.PricingOptions, x => x.OwnsMany(z => z.CategoryPricingOptions));

        builder.HasMany(x => x.Products).WithOne(x => x.Store).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Categories).WithOne(x => x.Store).OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(x => x.Attributes).WithOne(x => x.Store).OnDelete(DeleteBehavior.Cascade);

        //builder.HasMany(x => x.Images).WithOne(x => x.Store).OnDelete(DeleteBehavior.Cascade);
    }
}