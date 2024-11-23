
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record UpdateProjectCommand(string OrganizationId, string ProjectId, string Name, string? Description) : IRequest<Result<ProjectDto>>
{
    public class UpdateProjectCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return new ProjectNotFound(request.ProjectId);
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Organization = await context.Organizations.FirstAsync(o => o.Id == request.OrganizationId);

            await context.SaveChangesAsync(cancellationToken);

            return project.ToDto();
        }
    }
}