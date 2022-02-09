using Skynet.TimeReport.Application.Activities;
using Skynet.TimeReport.Application.Projects;

namespace Skynet.TimeReport.Application.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, ActivityDto Activity, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);