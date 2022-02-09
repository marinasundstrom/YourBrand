
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Commands;

public class DeleteProjectMembershipCommand : IRequest
{
    public DeleteProjectMembershipCommand(string projectId, string membershipId)
    {
        ProjectId = projectId;
        MembershipId = membershipId;
    }

    public string ProjectId { get; }

    public string MembershipId { get; }

    public class DeleteProjectMembershipCommandHandler : IRequestHandler<DeleteProjectMembershipCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteProjectMembershipCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProjectMembershipCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
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

            _context.ProjectMemberships.Remove(m);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}