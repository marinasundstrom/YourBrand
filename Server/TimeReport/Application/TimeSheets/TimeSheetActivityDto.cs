using TimeReport.Application.Projects;

namespace TimeReport.Application.TimeSheets;

public record class TimeSheetActivityDto(string Id, string Name, string? Description, ProjectDto Project, IEnumerable<TimeSheetEntryDto> Entries);