
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectMembershipQuery(string OrganizationId, string ProjectId, string MembershipId) : IRequest<Result<ProjectMembershipDto>>
{
    public class GetProjectMembershipQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectMembershipQuery, Result<ProjectMembershipDto>>
    {
        public async Task<Result<ProjectMembershipDto>> Handle(GetProjectMembershipQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(x => x.Organization)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                return new ProjectMembershipNotFound(request.ProjectId);
            }

            return m.ToDto();
        }
    }
}