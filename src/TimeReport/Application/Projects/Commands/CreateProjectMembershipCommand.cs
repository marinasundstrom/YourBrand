
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record CreateProjectMembershipCommand(string ProjectId, string UserId, DateTime? From, DateTime? To) : IRequest<ProjectMembershipDto>
{
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
                        .Include(p => p.Organization)
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

            return m.ToDto();
        }
    }
}