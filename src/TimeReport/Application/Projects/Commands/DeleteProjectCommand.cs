
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record DeleteProjectCommand(string OrganizationId, string ProjectId) : IRequest<Result>
{
    public class DeleteProjectCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteProjectCommand, Result>
    {
        public async Task<Result> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            context.Projects.Remove(project);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}