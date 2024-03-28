using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes;

public record class ActivityTypeDto(string Id, string Name, string? Description, bool ExcludeHours, ProjectDto? Project);