using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IAccountingContext
{
    DbSet<Account> Accounts { get; }
    DbSet<LedgerEntry> LedgerEntries { get; }
    DbSet<JournalEntry> JournalEntries { get; }
    DbSet<Verification> Verifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}