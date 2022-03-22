using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, ActivityDto Activity, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);