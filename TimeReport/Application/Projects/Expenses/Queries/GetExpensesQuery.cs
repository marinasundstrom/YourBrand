
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Queries;

public class GetExpensesQuery : IRequest<ItemsResult<ExpenseDto>>
{
    public GetExpensesQuery(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
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