
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Queries;

public record GetExpenseTypeQuery(string ExpenseId) : IRequest<ExpenseTypeDto>
{
    public class GetExpenseQueryHandler : IRequestHandler<GetExpenseTypeQuery, ExpenseTypeDto>
    {
        private readonly ITimeReportContext _context;

        public GetExpenseQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ExpenseTypeDto> Handle(GetExpenseTypeQuery request, CancellationToken cancellationToken)
        {
            var expenseType = await _context.ExpenseTypes
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