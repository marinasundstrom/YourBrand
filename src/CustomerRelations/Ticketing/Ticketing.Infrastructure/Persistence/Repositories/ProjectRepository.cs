using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Domain.Specifications;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(ApplicationDbContext context) : IProjectRepository
{
    public void Add(Project entity)
    {
        throw new NotImplementedException();
    }

    public async Task<Project?> FindByIdAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
                .Include(x => x.Memberships)
                .ThenInclude(x => x.User)
                //.Include(x => x.Organization)
                //.Include(x => x.Tickets)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Project> GetAll()
    {
        return context.Projects
                .Include(x => x.Memberships)
                .ThenInclude(x => x.User)
                //.Include(x => x.Organization)
                //.Include(x => x.Tickets)
                .AsQueryable();
    }

    public IQueryable<Project> GetAll(ISpecification<Project> specification)
    {
        throw new NotImplementedException();
    }

    public void Remove(Project entity)
    {
        throw new NotImplementedException();
    }
}