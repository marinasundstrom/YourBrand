
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes.Queries;

public record GetTaskTypesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TaskTypeDto>>
{
    public class GetTasksQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskTypesQuery, ItemsResult<TaskTypeDto>>
    {
        public async Task<ItemsResult<TaskTypeDto>> Handle(GetTaskTypesQuery request, CancellationToken cancellationToken)
        {
            var query = context.TaskTypes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(taskType => taskType.Project.Id == request.ProjectId);
            }

            /*
            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }
            */

            if (request.SearchString is not null)
            {
                query = query.Where(task => task.Name.ToLower().Contains(request.SearchString.ToLower()) || task.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var taskTypes = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = taskTypes.Select(taskType => taskType.ToDto());

            return new ItemsResult<TaskTypeDto>(dtos, totalItems);
        }
    }
}