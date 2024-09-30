
using MediatR;

namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Commands;

public record CreateProjectGroupCommand(string OrganizationId, string Name, string? Description) : IRequest<ProjectGroupDto>
{
    public class CreateExpenseCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateProjectGroupCommand, ProjectGroupDto>
    {
        public async Task<ProjectGroupDto> Handle(CreateProjectGroupCommand request, CancellationToken cancellationToken)
        {
            /*
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }
            */

            var projectGroup = new ProjectGroup(Guid.NewGuid().ToString())
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Description = request.Description,
                //Project = project
            };

            context.ProjectGroups.Add(projectGroup);

            await context.SaveChangesAsync(cancellationToken);

            return projectGroup.ToDto();
        }
    }
}