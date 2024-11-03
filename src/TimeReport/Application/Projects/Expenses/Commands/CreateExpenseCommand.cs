
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Expenses.Commands;

public record CreateExpenseCommand(string OrganizationId, string ProjectId, DateTime Date, string ExpenseTypeId, decimal Amount, string? Description) : IRequest<ExpenseDto>
{
    public class CreateExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<CreateExpenseCommand, ExpenseDto>
    {
        public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .InOrganization(request.OrganizationId)
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var expense = new Expense
            {
                OrganizationId = request.OrganizationId,
                ExpenseType = await context.ExpenseTypes.FirstAsync(et => et.Id == request.ExpenseTypeId),
                Date = DateOnly.FromDateTime(request.Date),
                Amount = request.Amount,
                Description = request.Description,
                Project = project
            };

            context.Expenses.Add(expense);

            await context.SaveChangesAsync(cancellationToken);

            return expense.ToDto();
        }
    }
}