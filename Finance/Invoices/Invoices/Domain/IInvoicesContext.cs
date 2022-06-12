using YourBrand.Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Domain;

public interface IInvoicesContext
{
    DbSet<Invoice> Invoices { get; }

    DbSet<InvoiceItem> InvoiceItems { get; }

    DbSet<RotRutRequest> RotRutRequests { get; set; }

    DbSet<RotRutCase> RotRutCases { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}