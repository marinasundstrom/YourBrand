using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain.Entities;

namespace YourBrand.Transactions.Domain;

public interface ITransactionsContext
{
    DbSet<Transaction> Transactions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}