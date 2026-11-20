using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IAccountingContext
{
    DbSet<Account> Accounts { get; }
    DbSet<LedgerEntry> LedgerEntries { get; }
    DbSet<Verification> Verifications { get; }
    DbSet<SupportingDocument> SupportingDocuments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}