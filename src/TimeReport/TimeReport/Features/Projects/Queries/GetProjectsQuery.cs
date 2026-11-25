
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Projects.Queries;

public record GetProjectsQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? UserId = null, string? SearchString = null, string? SortBy = null, TimeReport.Features.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProjectDto>>
{
    public class GetProjectsQueryHandler(ITimeReportContext context) : IRequestHandler<GetProjectsQuery, ItemsResult<ProjectDto>>
    {
        public async Task<ItemsResult<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Projects
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.UserId is not null)
            {
                query = query.Where(project => project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }

            if (request.SearchString is not null)
            {
                query = query.Where(project => project.Name.ToLower().Contains(request.SearchString.ToLower()) || project.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Features.Common.Models.SortDirection.Desc ? TimeReport.Features.SortDirection.Descending : TimeReport.Features.SortDirection.Ascending);
            }

            var projects = await query
                .Include(p => p.Organization)
                .Include(p => p.Memberships)
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync();

            var dtos = projects.Select(project => project.ToDto());

            return new ItemsResult<ProjectDto>(dtos, totalItems);
        }
    }
}