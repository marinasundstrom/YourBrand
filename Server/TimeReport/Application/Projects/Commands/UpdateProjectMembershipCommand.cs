
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Users;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Commands;

public class UpdateProjectMembershipCommand : IRequest<ProjectMembershipDto>
{
    public UpdateProjectMembershipCommand(string projectId, string membershipId, DateTime? from, DateTime? thru)
    {
        ProjectId = projectId;
        MembershipId = membershipId;
        From = from;
        Thru = thru;
    }

    public string ProjectId { get; }

    public string MembershipId { get; }

    public DateTime? From { get; }

    public DateTime? Thru { get; }

    public class UpdateProjectMembershipCommandHandler : IRequestHandler<UpdateProjectMembershipCommand, ProjectMembershipDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateProjectMembershipCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectMembershipDto> Handle(UpdateProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .Include(p => p.Memberships)
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
            m.Thru = request.Thru;

            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectMembershipDto(m.Id, new ProjectDto(m.Project.Id, m.Project.Name, m.Project.Description),
                new UserDto(m.User.Id, m.User.FirstName, m.User.LastName, m.User.DisplayName, m.User.SSN, m.User.Email, m.User.Created, m.User.Deleted),
                m.From, m.Thru);
        }
    }
}