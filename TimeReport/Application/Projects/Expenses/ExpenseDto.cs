namespace YourBrand.TimeReport.Application.Projects.Expenses;

public record class ExpenseDto(string Id, DateTime Date, decimal Amount, string? Description, string? Attachment, ProjectDto Project);