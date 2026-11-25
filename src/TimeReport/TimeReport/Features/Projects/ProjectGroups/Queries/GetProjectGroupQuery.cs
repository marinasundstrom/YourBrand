
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Projects.ProjectGroups.Queries;

public record GetProjectGroupQuery(string OrganizationId, string ExpenseId) : IRequest<ProjectGroupDto>
{
    public class GetExpenseQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectGroupQuery, ProjectGroupDto>
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