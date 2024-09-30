
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record CreateProjectCommand(string OrganizationId, string Name, string? Description) : IRequest<Result<ProjectDto>>
{
    public class CreateProjectCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            int projectId = 1;

            try
            {
                projectId = await context.Projects
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var project = new Project(projectId, request.Name, request.Description);
            project.OrganizationId = request.OrganizationId;

            context.Projects.Add(project);

            await context.SaveChangesAsync(cancellationToken);

            project = await context.Projects
                .FirstAsync(x => x.Id == project.Id);

            return project.ToDto();
        }
    }
}