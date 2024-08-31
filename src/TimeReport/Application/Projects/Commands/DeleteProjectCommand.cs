
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record DeleteProjectCommand(string OrganizationId, string ProjectId) : IRequest
{
    public class DeleteProjectCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteProjectCommand>
    {
        public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            context.Projects.Remove(project);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}