namespace YourBrand.TimeReport.Features.Projects.Expenses.ExpenseTypes;

public record class ExpenseTypeDto(string Id, string Name, string? Description, ProjectDto? Project);