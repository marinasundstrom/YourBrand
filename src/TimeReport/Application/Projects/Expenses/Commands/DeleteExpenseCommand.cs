
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public record DeleteExpenseCommand(string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteExpenseCommand>
    {
        public async Task Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await context.Expenses
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expense is null)
            {
                throw new Exception();
            }

            context.Expenses.Remove(expense);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}