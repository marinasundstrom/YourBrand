
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Queries;

public record GetExpenseQuery(string ExpenseId) : IRequest<ExpenseDto>
{
    public class GetExpenseQueryHandler(ITimeReportContext context) : IRequestHandler<GetExpenseQuery, ExpenseDto>
    {
        public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
        {
            var expense = await context.Expenses
               .Include(x => x.Project)
               .Include(x => x.ExpenseType)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            return expense.ToDto();
        }
    }
}