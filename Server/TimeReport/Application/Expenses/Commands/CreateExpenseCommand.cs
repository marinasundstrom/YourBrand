
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Projects;
using YourCompany.TimeReport.Domain.Entities;

using static YourCompany.TimeReport.Application.Expenses.ExpensesHelpers;

namespace YourCompany.TimeReport.Application.Expenses.Commands;

public class CreateExpenseCommand : IRequest<ExpenseDto>
{
    public CreateExpenseCommand(string projectId, DateTime date, decimal amount, string? description)
    {
        ProjectId = projectId;
        Date = date;
        Amount = amount;
        Description = description;
    }

    public string ProjectId { get; }

    public DateTime Date { get; }

    public decimal Amount { get; }

    public string? Description { get; }

    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, ExpenseDto>
    {
        private readonly ITimeReportContext _context;

        public CreateExpenseCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ExpenseDto> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var expense = new Expense
            {
                Id = Guid.NewGuid().ToString(),
                Type = ExpenseType.Purchase,
                Date = DateOnly.FromDateTime(request.Date),
                Amount = request.Amount,
                Description = request.Description,
                Project = project
            };

            _context.Expenses.Add(expense);

            await _context.SaveChangesAsync(cancellationToken);

            return new ExpenseDto(expense.Id, expense.Date.ToDateTime(TimeOnly.Parse("1:00")), expense.Amount, expense.Description, GetAttachmentUrl(expense.Attachment), new ProjectDto(expense.Project.Id, expense.Project.Name, expense.Project.Description));
        }
    }
}