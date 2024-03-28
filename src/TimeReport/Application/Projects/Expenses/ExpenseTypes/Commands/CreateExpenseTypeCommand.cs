
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes.Commands;

public record CreateExpenseTypeCommand(string Name, string? Description) : IRequest<ExpenseTypeDto>
{
    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseTypeCommand, ExpenseTypeDto>
    {
        private readonly ITimeReportContext _context;

        public CreateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

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
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                //Project = project
            };

            _context.ExpenseTypes.Add(expenseType);

            await _context.SaveChangesAsync(cancellationToken);

            return expenseType.ToDto();
        }
    }
}