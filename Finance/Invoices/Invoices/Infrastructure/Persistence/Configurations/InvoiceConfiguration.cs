using YourBrand.Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Invoices.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.OwnsOne(x => x.DomesticService, e => e.OwnsOne(z => z.PropertyDetails));

        builder.Ignore(e => e.DomainEvents);
    }
}
