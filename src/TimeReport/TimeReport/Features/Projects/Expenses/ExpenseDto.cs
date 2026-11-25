using YourBrand.TimeReport.Features.Projects.Expenses.ExpenseTypes;

namespace YourBrand.TimeReport.Features.Projects.Expenses;

public record class ExpenseDto(string Id, DateTime Date, ExpenseTypeDto ExpenseType, decimal Amount, string? Description, string? Attachment, ProjectDto Project);