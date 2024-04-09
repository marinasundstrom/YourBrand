using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class ProjectRepository(TimeReportContext context) : IProjectRepository
{
    public async Task<Project?> GetProject(string id, CancellationToken cancellationToken = default)
    {
        return await context.Projects
                .Include(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.ActivityType)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Project> GetProjects()
    {
        return context.Projects
                .Include(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.ActivityType)
                .AsQueryable();
    }
}