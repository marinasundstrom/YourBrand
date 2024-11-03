
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record CreateExpenseTypeCommand(string OrganizationId, string Name, string? Description) : IRequest<ExpenseTypeDto>
{
    public class CreateExpenseCommandHandler(ITimeReportContext context) : IRequestHandler<CreateExpenseTypeCommand, ExpenseTypeDto>
    {
        public async Task<ExpenseTypeDto> Handle(CreateExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            /*
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }
            */

            var expenseType = new ExpenseType
            {
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Description = request.Description,
                //Project = project
            };

            context.ExpenseTypes.Add(expenseType);

            await context.SaveChangesAsync(cancellationToken);

            return expenseType.ToDto();
        }
    }
}