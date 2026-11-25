
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;
using YourBrand.TimeReport.Features.Common.Models;

namespace YourBrand.TimeReport.Features.Projects.Expenses.Queries;

public record GetExpensesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, TimeReport.Features.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ExpenseDto>>
{
    public class GetExpensesQueryHandler(ITimeReportContext context) : IRequestHandler<GetExpensesQuery, ItemsResult<ExpenseDto>>
    {
        public async Task<ItemsResult<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var query = context.Expenses
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .Include(x => x.ExpenseType)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.ProjectId is not null)
            {
                query = query.Where(expense => expense.Project.Id == request.ProjectId);
            }

            if (request.SearchString is not null)
            {
                query = query.Where(expense => expense.Description.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Features.Common.Models.SortDirection.Desc ? TimeReport.Features.SortDirection.Descending : TimeReport.Features.SortDirection.Ascending);
            }

            var expenses = await query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = expenses.Select(expense => expense.ToDto());

            return new ItemsResult<ExpenseDto>(dtos, totalItems);
        }
    }
}