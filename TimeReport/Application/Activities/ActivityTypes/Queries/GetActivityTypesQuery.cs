
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Queries;

public class GetActivityTypesQuery : IRequest<ItemsResult<ActivityTypeDto>>
{
    public GetActivityTypesQuery(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        ProjectId = projectId;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }

    public int PageSize { get; }

    public string? ProjectId { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

    public class GetActivitiesQueryHandler : IRequestHandler<GetActivityTypesQuery, ItemsResult<ActivityTypeDto>>
    {
        private readonly ITimeReportContext _context;

        public GetActivitiesQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ActivityTypeDto>> Handle(GetActivityTypesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.ActivityTypes
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(activityType => activityType.Project.Id == request.ProjectId);
            }

            /*
            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }
            */

            if (request.SearchString is not null)
            {
                query = query.Where(activity => activity.Name.ToLower().Contains(request.SearchString.ToLower()) || activity.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var activityTypes = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = activityTypes.Select(activityType => activityType.ToDto());

            return new ItemsResult<ActivityTypeDto>(dtos, totalItems);
        }
    }
}