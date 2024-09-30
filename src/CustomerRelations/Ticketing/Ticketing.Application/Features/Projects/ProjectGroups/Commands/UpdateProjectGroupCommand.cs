
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Commands;

public record UpdateProjectGroupCommand(string OrganizationId, string ExpenseId, string Name, string? Description) : IRequest<ProjectGroupDto>
{
    public class UpdateExpenseCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateProjectGroupCommand, ProjectGroupDto>
    {
        public async Task<ProjectGroupDto> Handle(UpdateProjectGroupCommand request, CancellationToken cancellationToken)
        {
            var projectGroup = await context.ProjectGroups
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            projectGroup.Name = request.Name;
            projectGroup.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return projectGroup.ToDto();
        }
    }
}