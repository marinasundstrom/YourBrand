namespace YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes;

public record class ExpenseTypeDto(string Id, string Name, string? Description, ProjectDto? Project);