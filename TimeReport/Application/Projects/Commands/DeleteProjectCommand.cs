
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record DeleteProjectCommand(string ProjectId) : IRequest
{
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