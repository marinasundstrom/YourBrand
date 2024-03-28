using YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes;

namespace YourBrand.TimeReport.Application.Projects.Expenses;

public record class ExpenseDto(string Id, DateTime Date, ExpenseTypeDto ExpenseType, decimal Amount, string? Description, string? Attachment, ProjectDto Project);