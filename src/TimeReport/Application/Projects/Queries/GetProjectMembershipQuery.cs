
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Queries;

public record GetProjectMembershipQuery(string OrganizationId, string ProjectId, string MembershipId) : IRequest<ProjectMembershipDto>
{
    public class GetProjectMembershipQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectMembershipQuery, ProjectMembershipDto>
    {
        public async Task<ProjectMembershipDto> Handle(GetProjectMembershipQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .Include(x => x.Organization)
                .Include(p => p.Memberships)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                throw new ProjectMembershipNotFoundException(request.ProjectId);
            }

            return m.ToDto();
        }
    }
}