using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ChatApp.Domain.Specifications;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.Repositories;

public sealed class CachedUserRepository : IUserRepository
{
    private readonly IUserRepository decorated;
    private readonly IMemoryCache memoryCache;

    public CachedUserRepository(IUserRepository decorated, IMemoryCache memoryCache)
    {
        this.decorated = decorated;
        this.memoryCache = memoryCache;
    }

    public void Add(User item)
    {
        decorated.Add(item);
    }

    public async Task<User?> FindByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        string key = $"user-{id}";

        return await memoryCache.GetOrCreateAsync<User?>(key, async options =>
        {
            options.AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(2);

            return await decorated.FindByIdAsync(id, cancellationToken);
        });
    }

    public IQueryable<User> GetAll()
    {
        return decorated.GetAll();
    }

    public IQueryable<User> GetAll(ISpecification<User> specification)
    {
        return decorated.GetAll(specification);
    }

    public void Remove(User item)
    {
        decorated.Remove(item);
    }
}
