using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Domain.Entities.Country>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Country> builder)
    {
        builder.ToTable("Countries");

        builder.HasKey(x => x.Code);

        builder.HasOne(x => x.Continent)
                .WithMany()
                .HasForeignKey("ContinentCode")
                .IsRequired(false);

        builder.HasMany(e => e.Languages)
            .WithMany(e => e.Countries)
            .UsingEntity<CountryLanguage>(
                l => l.HasOne<Language>(e => e.Language).WithMany(e => e.CountryLanguages).HasForeignKey(x => x.LanguageCode),
                r => r.HasOne<Country>(e => e.Country).WithMany(e => e.CountryLanguages).HasForeignKey(x => x.CountryCode));

        builder.HasMany(e => e.Currencies)
            .WithMany(e => e.Countries)
            .UsingEntity<CountryCurrency>(
                l => l.HasOne<Currency>(e => e.Currency).WithMany(e => e.CountryCurrencies).HasForeignKey(x => x.CurrencyCode),
                r => r.HasOne<Country>(e => e.Country).WithMany(e => e.CountryCurrencies).HasForeignKey(x => x.CountryCode));
        /*
        builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey("CurrencyCode")
                .IsRequired(false);

        builder.HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey("LanguageCode")
                .IsRequired(false);
        */

        builder.HasMany(x => x.Regions)
                .WithOne(x => x.Country)
                .IsRequired(false);
    }
}

public class CountryLanguageConfiguration : IEntityTypeConfiguration<Domain.Entities.CountryLanguage>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CountryLanguage> builder)
    {
        builder.ToTable("CountryLanguages");

        /*

        builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey("CountryCode");

        builder.HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey("LanguageCode");

        */
    }
}

public class CountryCurrencyConfiguration : IEntityTypeConfiguration<Domain.Entities.CountryCurrency>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.CountryCurrency> builder)
    {
        builder.ToTable("CountryCurrencies");

        /*

        builder.HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey("CountryCode");

        builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey("CurrencyCode");

                */
    }
}