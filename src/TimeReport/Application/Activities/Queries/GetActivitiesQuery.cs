
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Activities.Queries;

public record GetActivitiesQuery(int Page = 0, int PageSize = 10, string? ProjectId = null, string? UserId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ActivityDto>>
{
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
                .Include(x => x.ActivityType)
                .Include(x => x.Project)
                .ThenInclude(x => x.Organization)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(activity => activity.Project.Id == request.ProjectId);
            }

            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
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

            var dtos = activities.Select(activity => activity.ToDto());

            return new ItemsResult<ActivityDto>(dtos, totalItems);
        }
    }
}