using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class VatRateConfiguration : IEntityTypeConfiguration<Domain.Entities.VatRate>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.VatRate> builder)
    {
        builder.ToTable("VatRates");
    }
}