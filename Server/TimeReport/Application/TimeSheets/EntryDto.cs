using YourCompany.TimeReport.Application.Activities;
using YourCompany.TimeReport.Application.Projects;

namespace YourCompany.TimeReport.Application.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, ActivityDto Activity, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);