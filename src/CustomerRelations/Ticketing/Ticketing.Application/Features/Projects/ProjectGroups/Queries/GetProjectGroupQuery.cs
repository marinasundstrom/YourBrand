
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Queries;

public record GetProjectGroupQuery(string OrganizationId, string ExpenseId) : IRequest<ProjectGroupDto>
{
    public class GetExpenseQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProjectGroupQuery, ProjectGroupDto>
    {
        public async Task<ProjectGroupDto> Handle(GetProjectGroupQuery request, CancellationToken cancellationToken)
        {
            var projectGroup = await context.ProjectGroups
                .InOrganization(request.OrganizationId)
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (projectGroup is null)
            {
                throw new Exception();
            }

            return projectGroup.ToDto();
        }
    }
}