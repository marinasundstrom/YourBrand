using Microsoft.EntityFrameworkCore;

using Transactions.Domain.Entities;

namespace Transactions.Domain;

public interface ITransactionsContext
{
    DbSet<Transaction> Transactions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}