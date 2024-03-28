
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record UpdateExpenseTypeCommand(string ExpenseId, string Name, string? Description) : IRequest<ExpenseTypeDto>
{
    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseTypeCommand, ExpenseTypeDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ExpenseTypeDto> Handle(UpdateExpenseTypeCommand request, CancellationToken cancellationToken)
        {
            var expenseType = await _context.ExpenseTypes
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ExpenseId, cancellationToken);

            if (expenseType is null)
            {
                throw new Exception();
            }

            expenseType.Name = request.Name;
            expenseType.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return expenseType.ToDto();
        }
    }
}