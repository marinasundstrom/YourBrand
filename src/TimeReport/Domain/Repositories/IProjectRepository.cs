using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IProjectRepository
{
    IQueryable<Project> GetProjects();

    Task<Project?> GetProject(string id, CancellationToken cancellationToken = default);
}
