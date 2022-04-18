using YourBrand.ApiKeys.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IApiKeysContext
{
    DbSet<ApiKey> ApiKeys { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync();
}

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}