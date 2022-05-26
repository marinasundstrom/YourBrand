using Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Invoices.Domain;

public interface IInvoicesContext
{
    DbSet<Invoice> Invoices { get; }

    DbSet<InvoiceItem> InvoiceItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}