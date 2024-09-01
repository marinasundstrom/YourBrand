
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public record UpdateExpenseCommand(string OrganizationId, string ExpenseId, DateTime Date, decimal Amount, string? Description) : IRequest<ExpenseDto>
{
    public class UpdateExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateExpenseCommand, ExpenseDto>
    {
        public async Task<ExpenseDto> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await context.Expenses
                .InOrganization(request.OrganizationId)
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

            await context.SaveChangesAsync(cancellationToken);

            return expense.ToDto();
        }
    }
}