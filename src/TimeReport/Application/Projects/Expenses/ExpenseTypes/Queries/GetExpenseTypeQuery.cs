
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Queries;

public record GetExpenseTypeQuery(string OrganizationId, string ExpenseId) : IRequest<ExpenseTypeDto>
{
    public class GetExpenseQueryHandler(ITimeReportContext context) : IRequestHandler<GetExpenseTypeQuery, ExpenseTypeDto>
    {
        public async Task<ExpenseTypeDto> Handle(GetExpenseTypeQuery request, CancellationToken cancellationToken)
        {
            var expenseType = await context.ExpenseTypes
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expenseType is null)
            {
                throw new Exception();
            }

            return expenseType.ToDto();
        }
    }
}