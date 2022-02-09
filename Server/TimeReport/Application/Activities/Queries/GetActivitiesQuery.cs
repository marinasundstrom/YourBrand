
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Application.Projects;

namespace TimeReport.Application.Activities.Queries;

public class GetActivitiesQuery : IRequest<ItemsResult<ActivityDto>>
{
    public GetActivitiesQuery(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
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

    public class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, ItemsResult<ActivityDto>>
    {
        private readonly ITimeReportContext _context;

        public GetActivitiesQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ActivityDto>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Activities
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(activity => activity.Project.Id == request.ProjectId);
            }

            if (request.SearchString is not null)
            {
                query = query.Where(activity => activity.Name.ToLower().Contains(request.SearchString.ToLower()) || activity.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var activities = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = activities.Select(activity => new ActivityDto(activity.Id, activity.Name, activity.Description, activity.HourlyRate, new ProjectDto(activity.Project.Id, activity.Project.Name, activity.Project.Description)));

            return new ItemsResult<ActivityDto>(dtos, totalItems);
        }
    }
}