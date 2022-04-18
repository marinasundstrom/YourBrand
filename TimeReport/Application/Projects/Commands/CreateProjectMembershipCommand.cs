
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public partial class CreateProjectMembershipCommand : IRequest<ProjectMembershipDto>
{
    public CreateProjectMembershipCommand(string projectId, string userId, DateTime? from, DateTime? to)
    {
        ProjectId = projectId;
        UserId = userId;
        From = from;
        To = to;
    }

    public string ProjectId { get; }

    public string UserId { get; }

    public DateTime? From { get; }

    public DateTime? To { get; }

    public class CreateProjectMembershipCommandHandler : IRequestHandler<CreateProjectMembershipCommand, ProjectMembershipDto>
    {
        private readonly ITimeReportContext _context;

        public CreateProjectMembershipCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ProjectMembershipDto> Handle(CreateProjectMembershipCommand request, CancellationToken cancellationToken)
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

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            var membership = project.Memberships.FirstOrDefault(x => x.User.Id == user.Id);

            if (membership is not null)
            {
                throw new UserAlreadyProjectMemberException(request.UserId, request.ProjectId);
            }

            var m = new ProjectMembership()
            {
                Id = Guid.NewGuid().ToString(),
                Project = project,
                User = user,
                From = request.From,
                To = request.To
            };

            _context.ProjectMemberships.Add(m);

            await _context.SaveChangesAsync(cancellationToken);

            return new ProjectMembershipDto(m.Id, new ProjectDto(m.Project.Id, m.Project.Name, m.Project.Description),
                new UserDto(m.User.Id, m.User.FirstName, m.User.LastName, m.User.DisplayName, m.User.SSN, m.User.Email, m.User.Created, m.User.Deleted),
                m.From, m.To);
        }
    }
}