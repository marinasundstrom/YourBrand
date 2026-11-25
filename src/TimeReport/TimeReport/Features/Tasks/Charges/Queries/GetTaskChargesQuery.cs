
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Tasks.TaskCharges.Queries;

public record GetTaskChargesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? TaskId = null, string? SearchString = null, string? SortBy = null, TimeReport.Features.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TaskChargeDto>>
{
    public class GetTasksQueryHandler(ITimeReportContext context) : IRequestHandler<GetTaskChargesQuery, ItemsResult<TaskChargeDto>>
    {
        public async Task<ItemsResult<TaskChargeDto>> Handle(GetTaskChargesQuery request, CancellationToken cancellationToken)
        {
            var query = context.TaskCharges
                .InOrganization(request.OrganizationId)
                .Include(x => x.Task)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(taskCharge => taskCharge.Project.Id == request.ProjectId);
            }

            if (request.TaskId is not null)
            {
                query = query.Where(taskCharge => taskCharge.Task.Id == request.TaskId);
            }

            /*
            if (request.UserId is not null)
            {
                query = query.Where(a => a.Project.Memberships.Any(pm => pm.User.Id == request.UserId));
            }
            */

            if (request.SearchString is not null)
            {
                query = query.Where(task => task.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Features.Common.Models.SortDirection.Desc ? TimeReport.Features.SortDirection.Descending : TimeReport.Features.SortDirection.Ascending);
            }

            var taskCharges = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = taskCharges.Select(taskCharge => taskCharge.ToDto());

            return new ItemsResult<TaskChargeDto>(dtos, totalItems);
        }
    }
}