
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record UpdateProjectCommand(string OrganizationId, string ProjectId, string Name, string? Description) : IRequest<ProjectDto>
{
    public class UpdateProjectCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new ProjectNotFoundException(request.ProjectId);
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.Organization = await context.Organizations.FirstAsync(o => o.Id == request.OrganizationId);

            await context.SaveChangesAsync(cancellationToken);

            return project.ToDto();
        }
    }
}