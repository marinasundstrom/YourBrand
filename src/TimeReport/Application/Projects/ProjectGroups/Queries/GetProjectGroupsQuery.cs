
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups.Queries;

public record GetProjectGroupsQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProjectGroupDto>>
{
    public class GetActivitiesQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectGroupsQuery, ItemsResult<ProjectGroupDto>>
    {
        public async Task<ItemsResult<ProjectGroupDto>> Handle(GetProjectGroupsQuery request, CancellationToken cancellationToken)
        {
            var query = context.ProjectGroups
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(projectGroup => projectGroup.Project.Id == request.ProjectId);
            }

            /*
            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }
            */

            if (request.SearchString is not null)
            {
                query = query.Where(expense => expense.Name.ToLower().Contains(request.SearchString.ToLower()) || expense.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var projectGroups = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = projectGroups.Select(projectGroup => projectGroup.ToDto());

            return new ItemsResult<ProjectGroupDto>(dtos, totalItems);
        }
    }
}