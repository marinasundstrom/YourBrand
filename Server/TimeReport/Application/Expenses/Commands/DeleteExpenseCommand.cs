
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Application.Expenses.Commands;

public class DeleteExpenseCommand : IRequest
{
    public DeleteExpenseCommand(string expenseId)
    {
        ExpenseId = expenseId;
    }

    public string ExpenseId { get; }

    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _context.Expenses
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            _context.Expenses.Remove(expense);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}