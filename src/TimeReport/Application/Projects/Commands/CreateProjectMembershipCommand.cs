
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record CreateProjectMembershipCommand(string OrganizationId, string ProjectId, string UserId, DateTime? From, DateTime? To) : IRequest<Result<ProjectMembershipDto>>
{
    public class CreateProjectMembershipCommandHandler(ITimeReportContext context) : IRequestHandler<CreateProjectMembershipCommand, Result<ProjectMembershipDto>>
    {
        public async Task<Result<ProjectMembershipDto>> Handle(CreateProjectMembershipCommand request, CancellationToken cancellationToken)
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

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
               return new UserNotFound(request.UserId);
            }

            var membership = project.Memberships.FirstOrDefault(x => x.User.Id == user.Id);

            if (membership is not null)
            {
                return new UserAlreadyProjectMember(request.UserId, request.ProjectId);
            }

            var m = new ProjectMembership()
            {
                OrganizationId = request.OrganizationId,
                Project = project,
                User = user,
                From = request.From,
                To = request.To
            };

            context.ProjectMemberships.Add(m);

            await context.SaveChangesAsync(cancellationToken);

            return m.ToDto();
        }
    }
}