using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain.Specifications;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Infrastructure.Persistence.Repositories;

public sealed class MeetingRepository(ApplicationDbContext context) : IMeetingRepository
{
    readonly DbSet<Meeting> dbSet = context.Set<Meeting>();

    public IQueryable<Meeting> GetAll()
    {
        //return dbSet.Where(new MeetingsWithStatus(MeetingStatus.Completed).Or(new MeetingsWithStatus(MeetingStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<Meeting?> FindByIdAsync(MeetingId id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .Include(i => i.Attendees)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Meeting> GetAll(ISpecification<Meeting> specification)
    {
        return dbSet
            .Include(i => i.CreatedBy)
            .Include(i => i.LastModifiedBy)
            .Include(i => i.Attendees)
            .Where(specification.Criteria);
    }

    public void Add(Meeting item)
    {
        dbSet.Add(item);
    }

    public void Remove(Meeting item)
    {
        dbSet.Remove(item);
    }
}