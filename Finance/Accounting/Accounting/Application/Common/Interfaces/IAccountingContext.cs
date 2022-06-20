using YourBrand.Accounting.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IAccountingContext
{
    DbSet<Account> Accounts { get; }
    DbSet<Entry> Entries { get; }
    DbSet<Verification> Verifications { get; }
    DbSet<Attachment> Attachments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}