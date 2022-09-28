using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly TimeReportContext _context;

    public ProjectRepository(TimeReportContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetProject(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
                .Include(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.ActivityType)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Project> GetProjects()
    {
        return _context.Projects
                .Include(x => x.Organization)
                .Include(x => x.Activities)
                .ThenInclude(x => x.ActivityType)
                .AsQueryable();
    }
}
