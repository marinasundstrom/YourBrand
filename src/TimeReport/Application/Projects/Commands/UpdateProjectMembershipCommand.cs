
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record UpdateProjectMembershipCommand(string OrganizationId, string ProjectId, string MembershipId, DateTime? From, DateTime? To) : IRequest<ProjectMembershipDto>
{
    public class UpdateProjectMembershipCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateProjectMembershipCommand, ProjectMembershipDto>
    {
        public async Task<ProjectMembershipDto> Handle(UpdateProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .Include(p => p.Memberships)
                .ThenInclude(m => m.User)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            var m = project.Memberships.FirstOrDefault(x => x.Id == request.MembershipId);

            if (m is null)
            {
                throw new ProjectMembershipNotFoundException(request.MembershipId);
            }

            m.From = request.From;
            m.To = request.To;

            await context.SaveChangesAsync(cancellationToken);

            return m.ToDto();
        }
    }
}