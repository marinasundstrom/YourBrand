
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public class UpdateExpenseCommand : IRequest<ExpenseDto>
{
    public UpdateExpenseCommand(string expenseId, DateTime date, decimal amount, string? description)
    {
        ExpenseId = expenseId;
        Date = date;
        Amount = amount;
        Description = description;
    }

    public string ExpenseId { get; }

    public DateTime Date { get; }

    public decimal Amount { get; }

    public string? Description { get; }

    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, ExpenseDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ExpenseDto> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            expense.Date = DateOnly.FromDateTime(request.Date);
            expense.Amount = request.Amount;
            expense.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return expense.ToDto();
        }
    }
}