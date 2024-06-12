using Microsoft.EntityFrameworkCore;
using ChatApp.Domain.Specifications;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.Repositories;

public sealed class MessageRepository : IMessageRepository
{
    readonly ApplicationDbContext context;
    readonly DbSet<Message> dbSet;

    public MessageRepository(ApplicationDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<Message>();
    }

    public IQueryable<Message> GetAll()
    {
        return dbSet
            .IgnoreQueryFilters()
            .AsQueryable();
    }

    public async Task<Message?> FindByIdAsync(MessageId id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Message> GetAll(ISpecification<Message> specification)
    {
        return dbSet
            .IgnoreQueryFilters()
            .Where(specification.Criteria);
    }

    public void Add(Message item)
    {
        dbSet.Add(item);
    }

    public void Remove(Message item)
    {
        dbSet.Remove(item);
    }
}
