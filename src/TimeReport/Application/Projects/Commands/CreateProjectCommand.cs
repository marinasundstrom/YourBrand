
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Commands;

public record CreateProjectCommand(string OrganizationId, string Name, string? Description) : IRequest<ProjectDto>
{
    public class CreateProjectCommandHandler(ITimeReportContext context) : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = new Project(request.Name, request.Description);
            project.OrganizationId = request.OrganizationId;

            project.Organization = await context.Organizations.FirstAsync(x => x.Id == request.OrganizationId, cancellationToken);

            context.Projects.Add(project);

            await context.SaveChangesAsync(cancellationToken);

            project = await context.Projects
                .Include(x => x.Organization)
                .FirstAsync(x => x.Id == project.Id);

            return project.ToDto();
        }
    }
}