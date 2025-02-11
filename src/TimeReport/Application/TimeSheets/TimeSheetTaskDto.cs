using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.TimeSheets;

public record class TimeSheetTaskDto(string Id, string Name, string? Description, ProjectDto Project, IEnumerable<TimeSheetEntryDto> Entries);
