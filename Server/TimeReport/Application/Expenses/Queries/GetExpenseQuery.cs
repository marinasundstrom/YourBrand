
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;

using static TimeReport.Application.Expenses.ExpensesHelpers;

namespace TimeReport.Application.Expenses.Queries;

public class GetExpenseQuery : IRequest<ExpenseDto>
{
    public GetExpenseQuery(string expenseId)
    {
        ExpenseId = expenseId;
    }

    public string ExpenseId { get; }

    public class GetExpenseQueryHandler : IRequestHandler<GetExpenseQuery, ExpenseDto>
    {
        private readonly ITimeReportContext _context;

        public GetExpenseQueryHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ExpenseDto> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
               .Include(x => x.Project)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            return new ExpenseDto(expense.Id, expense.Date.ToDateTime(TimeOnly.Parse("1:00")), expense.Amount, expense.Description, GetAttachmentUrl(expense.Attachment), new ProjectDto(expense.Project.Id, expense.Project.Name, expense.Project.Description));
        }
    }
}