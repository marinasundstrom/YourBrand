using YourCompany.TimeReport.Application.Projects;

namespace YourCompany.TimeReport.Application.Expenses;

public record class ExpenseDto(string Id, DateTime Date, decimal Amount, string? Description, string? Attachment, ProjectDto Project);