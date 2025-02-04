using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Tasks.TaskTypes;

public record class TaskTypeDto(string Id, string Name, string? Description, bool ExcludeHours, ProjectDto? Project);