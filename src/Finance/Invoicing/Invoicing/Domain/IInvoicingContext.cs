using YourBrand.Invoicing.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoicing.Domain;

public interface IInvoicingContext
{
    DbSet<Invoice> Invoices { get; }

    DbSet<InvoiceItem> InvoiceItems { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}