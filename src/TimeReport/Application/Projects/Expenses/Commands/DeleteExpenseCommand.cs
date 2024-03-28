
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public record DeleteExpenseCommand(string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
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

        }
    }
}