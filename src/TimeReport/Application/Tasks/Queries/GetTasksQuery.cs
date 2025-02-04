
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Tasks.Queries;

public record GetTasksQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? UserId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TaskDto>>
{
    public class GetTasksQueryHandler(ITimeReportContext context) : IRequestHandler<GetTasksQuery, ItemsResult<TaskDto>>
    {
        public async Task<ItemsResult<TaskDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var query = context.Tasks
                .InOrganization(request.OrganizationId)
                .Include(x => x.TaskType)
                .Include(x => x.Project)
                .ThenInclude(x => x.Organization)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(task => task.Project.Id == request.ProjectId);
            }

            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }

            if (request.SearchString is not null)
            {
                query = query.Where(task => task.Name.ToLower().Contains(request.SearchString.ToLower()) || task.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var tasks = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = tasks.Select(task => task.ToDto());

            return new ItemsResult<TaskDto>(dtos, totalItems);
        }
    }
}