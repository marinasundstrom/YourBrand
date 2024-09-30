
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record UpdateProjectMembershipCommand(string OrganizationId, int ProjectId, string MembershipId, DateTime? From, DateTime? To) : IRequest<Result<ProjectMembershipDto>>
{
    public class UpdateProjectMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProjectMembershipCommand, Result<ProjectMembershipDto>>
    {
        public async Task<Result<ProjectMembershipDto>> Handle(UpdateProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return Errors.Projects.ProjectNotFound;
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                return Errors.Projects.ProjectMemberNotFound;
            }

            m.From = request.From;
            m.To = request.To;

            await context.SaveChangesAsync(cancellationToken);

            return m.ToDto();
        }
    }
}