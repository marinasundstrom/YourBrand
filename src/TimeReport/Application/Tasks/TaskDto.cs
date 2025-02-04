using YourBrand.TimeReport.Application.Tasks.TaskTypes;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Tasks;

public record class TaskDto(string Id, string Name, TaskTypeDto TaskType, string? Description, decimal? HourlyRate, ProjectDto? Project);