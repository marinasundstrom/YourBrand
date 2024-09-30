
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record DeleteProjectMembershipCommand(string OrganizationId, int ProjectId, string MembershipId) : IRequest<Result>
{
    public class DeleteProjectMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProjectMembershipCommand, Result>
    {
        public async Task<Result> Handle(DeleteProjectMembershipCommand request, CancellationToken cancellationToken)
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

            context.ProjectMemberships.Remove(m);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}