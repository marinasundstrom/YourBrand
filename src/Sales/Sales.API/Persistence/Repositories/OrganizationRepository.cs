using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Specifications;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.API.Persistence.Repositories.Mocks;

public sealed class OrganizationRepository : IOrganizationRepository
{
    readonly SalesContext context;
    readonly DbSet<Organization> dbSet;

    public OrganizationRepository(SalesContext context)
    {
        this.context = context;
        this.dbSet = context.Set<Organization>();
    }

    public IQueryable<Organization> GetAll()
    {
        //return dbSet.Where(new OrganizationsWithStatus(OrganizationStatus.Completed).Or(new OrganizationsWithStatus(OrganizationStatus.OnHold))).AsQueryable();

        return dbSet.AsQueryable();
    }

    public async Task<Organization?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await dbSet.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    public IQueryable<Organization> GetAll(ISpecification<Organization> specification)
    {
        return dbSet.Where(specification.Criteria);
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