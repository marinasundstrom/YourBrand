
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Queries;

public record GetExpenseTypesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ExpenseTypeDto>>
{
    public class GetActivitiesQueryHandler(ITimeReportContext context) : IRequestHandler<GetExpenseTypesQuery, ItemsResult<ExpenseTypeDto>>
    {
        public async Task<ItemsResult<ExpenseTypeDto>> Handle(GetExpenseTypesQuery request, CancellationToken cancellationToken)
        {
            var query = context.ExpenseTypes
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
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
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