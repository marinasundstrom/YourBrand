using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ContinentConfiguration : IEntityTypeConfiguration<Continent>
{
    public void Configure(EntityTypeBuilder<Continent> builder)
    {
        builder.ToTable("Continents");

        builder.HasKey(x => x.Code);
    }
}