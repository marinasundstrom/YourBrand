using YourCompany.TimeReport.Application.Projects;

namespace YourCompany.TimeReport.Application.Activities;

public record class ActivityDto(string Id, string Name, string? Description, decimal? HourlyRate, ProjectDto Project);