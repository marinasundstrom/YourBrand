
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Projects.Expenses.ExpenseTypes.Queries;

public record GetExpenseTypesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, TimeReport.Features.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ExpenseTypeDto>>
{
    public class GetTasksQueryHandler(ITimeReportContext context) : IRequestHandler<GetExpenseTypesQuery, ItemsResult<ExpenseTypeDto>>
    {
        public async Task<ItemsResult<ExpenseTypeDto>> Handle(GetExpenseTypesQuery request, CancellationToken cancellationToken)
        {
            var query = context.ExpenseTypes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(expenseType => expenseType.Project.Id == request.ProjectId);
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
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Features.Common.Models.SortDirection.Desc ? TimeReport.Features.SortDirection.Descending : TimeReport.Features.SortDirection.Ascending);
            }

            var expenseTypes = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = expenseTypes.Select(expenseType => expenseType.ToDto());

            return new ItemsResult<ExpenseTypeDto>(dtos, totalItems);
        }
    }
}