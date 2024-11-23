
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record DeleteProjectMembershipCommand(string OrganizationId, string ProjectId, string MembershipId) : IRequest<Result>
{
    public class DeleteProjectMembershipCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteProjectMembershipCommand, Result>
    {
        [Throws(typeof(OperationCanceledException))]
        public async Task<Result> Handle(DeleteProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(p => p.Organization)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                return new ProjectMembershipNotFound(request.MembershipId);
            }

            context.ProjectMemberships.Remove(m);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}