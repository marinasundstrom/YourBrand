using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Meetings.Domain.Specifications;

namespace YourBrand.Meetings.Infrastructure.Persistence.Repositories;

public sealed class OrganizationRepository(ApplicationDbContext context) : IOrganizationRepository
{
    readonly DbSet<Organization> dbSet = context.Set<Organization>();

    public IQueryable<Organization> GetAll()
    {
        //return dbSet.Where(new OrganizationsWithStatus(OrganizationStatus.Completed).Or(new OrganizationsWithStatus(OrganizationStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<Organization?> FindByIdAsync(OrganizationId id, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Include(x => x.Users)
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Organization> GetAll(ISpecification<Organization> specification)
    {
        return dbSet
            .Include(x => x.Users).Where(specification.Criteria);
    }

    public void Add(Organization organization)
    {
        dbSet.Add(organization);
    }

    public void Remove(Organization organization)
    {
        dbSet.Remove(organization);
    }
}