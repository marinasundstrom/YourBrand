
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Users;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Queries;

public class GetProjectMembershipQuery : IRequest<ProjectMembershipDto>
{
    public GetProjectMembershipQuery(string projectId, string membershipId)
    {
        ProjectId = projectId;
        MembershipId = membershipId;
    }

    public string ProjectId { get; }
    public string MembershipId { get; }

    public class GetProjectMembershipQueryHandler : IRequestHandler<GetProjectMembershipQuery, ProjectMembershipDto>
    {
        private readonly ITimeReportContext _context;

        public GetProjectMembershipQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectMembershipDto> Handle(GetProjectMembershipQuery request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
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

            return new ProjectMembershipDto(m.Id, new ProjectDto(m.Project.Id, m.Project.Name, m.Project.Description),
                new UserDto(m.User.Id, m.User.FirstName, m.User.LastName, m.User.DisplayName, m.User.SSN, m.User.Email, m.User.Created, m.User.Deleted),
                m.From, m.Thru);
        }
    }
}