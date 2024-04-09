
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record DeleteExpenseTypeCommand(string ExpenseId) : IRequest
{
    public class DeleteExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteExpenseTypeCommand>
    {
        public async Task Handle(DeleteExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            var expenseType = await context.ExpenseTypes
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expenseType is null)
            {
                throw new Exception();
            }

            context.ExpenseTypes.Remove(expenseType);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}