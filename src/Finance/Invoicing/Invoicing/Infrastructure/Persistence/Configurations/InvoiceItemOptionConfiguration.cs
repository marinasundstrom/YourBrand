using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Persistence.Configurations;

public sealed class InvoiceItemOptionConfiguration : IEntityTypeConfiguration<InvoiceItemOption>
{
    public void Configure(EntityTypeBuilder<InvoiceItemOption> builder)
    {
        builder.ToTable("InvoiceItemOptions");

        builder.HasKey(o => new { o.OrganizationId, o.InvoiceId, o.InvoiceItemId, o.Id });

        builder.Ignore(e => e.DomainEvents);
    }
}