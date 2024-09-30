
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record CreateProjectMembershipCommand(string OrganizationId, int ProjectId, string UserId, DateTime? From, DateTime? To) : IRequest<Result<ProjectMembershipDto>>
{
    public class CreateProjectMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProjectMembershipCommand, Result<ProjectMembershipDto>>
    {
        public async Task<Result<ProjectMembershipDto>> Handle(CreateProjectMembershipCommand request, CancellationToken cancellationToken)
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

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return Errors.Users.UserNotFound;
            }

            var membership = project.Memberships.FirstOrDefault(x => x.User.Id == user.Id);

            if (membership is not null)
            {
                return Errors.Projects.ProjectMemberNotFound;
            }

            var m = new ProjectMembership(Guid.NewGuid().ToString())
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