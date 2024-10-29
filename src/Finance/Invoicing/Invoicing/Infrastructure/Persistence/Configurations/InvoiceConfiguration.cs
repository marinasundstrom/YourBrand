using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(o => new { o.OrganizationId, o.Id });

        builder.HasIndex(o => new { o.OrganizationId, o.InvoiceNo });

        builder.HasOne(o => o.Status).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.StatusId });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(invoice => invoice.Items)
            .WithOne(invoiceItem => invoiceItem.Invoice)
            .HasForeignKey(o => new { o.OrganizationId, o.InvoiceId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.OwnsOne(x => x.Customer);

        builder.OwnsOne(x => x.BillingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsOne(x => x.ShippingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsMany(x => x.VatAmounts, e => e.ToJson());

        builder.OwnsOne(x => x.DomesticService, e => e.OwnsOne(z => z.PropertyDetails));
    }
}