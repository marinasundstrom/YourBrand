
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;

using static TimeReport.Application.Expenses.ExpensesHelpers;

namespace TimeReport.Application.Expenses.Commands;

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

            return new ExpenseDto(expense.Id, expense.Date.ToDateTime(TimeOnly.Parse("1:00")), expense.Amount, expense.Description, GetAttachmentUrl(expense.Attachment), new ProjectDto(expense.Project.Id, expense.Project.Name, expense.Project.Description));
        }
    }
}