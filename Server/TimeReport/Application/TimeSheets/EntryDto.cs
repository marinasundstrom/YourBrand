using TimeReport.Application.Activities;
using TimeReport.Application.Projects;

namespace TimeReport.Application.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, ActivityDto Activity, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);