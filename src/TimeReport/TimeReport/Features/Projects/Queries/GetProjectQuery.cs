
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Projects.Queries;

public record GetProjectQuery(string OrganizationId, string ProjectId) : IRequest<ProjectDto?>
{
    public class GetProjectQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectQuery, ProjectDto?>
    {
        public async Task<ProjectDto?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .InOrganization(request.OrganizationId)
               .Include(p => p.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return null;
            }

            return project.ToDto();
        }
    }
}