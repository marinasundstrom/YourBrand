using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain.Specifications;
using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Infrastructure.Persistence.Repositories;

public sealed class ChannelRepository : IChannelRepository
{
    readonly ApplicationDbContext context;
    readonly DbSet<Channel> dbSet;

    public ChannelRepository(ApplicationDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<Channel>();
    }

    public IQueryable<Channel> GetAll()
    {
        return dbSet.AsQueryable();
    }

    public async Task<Channel?> FindByIdAsync(ChannelId id, CancellationToken cancellationToken = default)
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Channel> GetAll(ISpecification<Channel> specification)
    {
        return dbSet.Where(specification.Criteria);
    }

    public void Add(Channel item)
    {
        dbSet.Add(item);
    }

    public void Remove(Channel item)
    {
        dbSet.Remove(item);
    }
}