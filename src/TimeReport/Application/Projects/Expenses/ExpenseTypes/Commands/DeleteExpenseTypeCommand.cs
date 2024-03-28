
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record DeleteExpenseTypeCommand(string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseTypeCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            var expenseType = await _context.ExpenseTypes
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expenseType is null)
            {
                throw new Exception();
            }

            _context.ExpenseTypes.Remove(expenseType);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}