using YourBrand.TimeReport.Features.Projects;

namespace YourBrand.TimeReport.Features.Tasks.TaskTypes;

public record class TaskTypeDto(string Id, string Name, string? Description, bool ExcludeHours, ProjectDto? Project);