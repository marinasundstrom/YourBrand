
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record UpdateExpenseTypeCommand(string OrganizationId, string ExpenseId, string Name, string? Description) : IRequest<ExpenseTypeDto>
{
    public class UpdateExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateExpenseTypeCommand, ExpenseTypeDto>
    {
        public async Task<ExpenseTypeDto> Handle(UpdateExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            var expenseType = await context.ExpenseTypes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expenseType is null)
            {
                throw new Exception();
            }

            expenseType.Name = request.Name;
            expenseType.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return expenseType.ToDto();
        }
    }
}