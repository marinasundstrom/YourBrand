using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Domain.Specifications;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository(ApplicationDbContext context) : ITicketRepository
{
    readonly DbSet<Ticket> dbSet = context.Set<Ticket>();

    public IQueryable<Ticket> GetAll()
    {
        //return dbSet.Where(new TicketsWithStatus(TicketStatus.Completed).Or(new TicketsWithStatus(TicketStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<Ticket?> FindByIdAsync(TicketId id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Include(i => i.Status)
            .Include(i => i.Type)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .Include(i => i.Participants)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Ticket> GetAll(ISpecification<Ticket> specification)
    {
        return dbSet
            .Include(i => i.Status)
            .Include(i => i.Type)
            .Include(i => i.Assignee)
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .Include(i => i.Participants)
            .Where(specification.Criteria);
    }

    public void Add(Ticket item)
    {
        dbSet.Add(item);
    }

    public void Remove(Ticket item)
    {
        dbSet.Remove(item);
    }
}