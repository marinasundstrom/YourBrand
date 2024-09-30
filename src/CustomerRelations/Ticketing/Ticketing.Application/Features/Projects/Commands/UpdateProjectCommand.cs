
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.Commands;

public record UpdateProjectCommand(string OrganizationId, int ProjectId, string Name, string? Description) : IRequest<Result<ProjectDto>>
{
    public class UpdateProjectCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                return Errors.Projects.ProjectNotFound;
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.OrganizationId = (await context.Organizations.FirstAsync(o => o.Id == request.OrganizationId)).Id;

            await context.SaveChangesAsync(cancellationToken);

            return project.ToDto();
        }
    }
}