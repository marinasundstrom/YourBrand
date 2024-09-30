
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Queries;

public record GetProjectQuery(string OrganizationId, int ProjectId) : IRequest<ProjectDto?>
{
    public class GetProjectQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProjectQuery, ProjectDto?>
    {
        public async Task<ProjectDto?> Handle(GetProjectQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .InOrganization(request.OrganizationId)
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