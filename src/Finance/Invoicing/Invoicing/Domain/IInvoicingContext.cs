using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Domain;

public interface IInvoicingContext
{
    DbSet<Invoice> Invoices { get; }

    DbSet<InvoiceStatus> InvoiceStatuses { get; }

    DbSet<InvoiceItem> InvoiceItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}