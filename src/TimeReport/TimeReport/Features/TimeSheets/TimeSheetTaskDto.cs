using YourBrand.TimeReport.Features.Projects;

namespace YourBrand.TimeReport.Features.TimeSheets;

public record class TimeSheetTaskDto(string Id, string Name, string? Description, ProjectDto Project, IEnumerable<TimeSheetEntryDto> Entries);
