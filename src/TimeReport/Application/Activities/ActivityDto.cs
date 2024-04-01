using YourBrand.TimeReport.Application.Activities.ActivityTypes;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Activities;

public record class ActivityDto(string Id, string Name, ActivityTypeDto ActivityType, string? Description, decimal? HourlyRate, ProjectDto? Project);