
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Queries;

public record GetProjectMembershipQuery(string OrganizationId, int ProjectId, string MembershipId) : IRequest<Result<ProjectMembershipDto>>
{
    public class GetProjectMembershipQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProjectMembershipQuery, Result<ProjectMembershipDto>>
    {
        public async Task<Result<ProjectMembershipDto>> Handle(GetProjectMembershipQuery request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                //.Include(x => x.Organization)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId);

            if (project is null)
            {
                return Errors.Projects.ProjectNotFound;
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                return Errors.Projects.ProjectMemberNotFound;
            }

            return m.ToDto();
        }
    }
}