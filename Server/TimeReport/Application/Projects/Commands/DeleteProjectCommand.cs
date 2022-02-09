
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.Projects.Commands;

public class DeleteProjectCommand : IRequest
{
    public DeleteProjectCommand(string projectId)
    {
        ProjectId = projectId;
    }

    public string ProjectId { get; }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteProjectCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            _context.Projects.Remove(project);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}