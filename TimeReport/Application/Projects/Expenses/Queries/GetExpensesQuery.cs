
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Queries;

public record GetExpensesQuery(int Page = 0, int PageSize = 10, string? ProjectId = null, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ExpenseDto>>
{
    public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, ItemsResult<ExpenseDto>>
    {
        private readonly ITimeReportContext _context;

        public GetExpensesQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Expenses
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
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
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