using YourCompany.TimeReport.Application.Projects;

namespace YourCompany.TimeReport.Application.TimeSheets;

public record class TimeSheetActivityDto(string Id, string Name, string? Description, ProjectDto Project, IEnumerable<TimeSheetEntryDto> Entries);