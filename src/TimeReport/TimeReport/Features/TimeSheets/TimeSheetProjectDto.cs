namespace YourBrand.TimeReport.Features.TimeSheets;

public record class TimeSheetProjectDto(string Id, IEnumerable<TimeSheetTaskDto> Tasks, IEnumerable<DaySummaryDto> DaySummaries);