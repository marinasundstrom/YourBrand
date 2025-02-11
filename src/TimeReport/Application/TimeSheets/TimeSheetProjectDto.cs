namespace YourBrand.TimeReport.Application.TimeSheets;

public record class TimeSheetProjectDto(string Id, IEnumerable<TimeSheetTaskDto> Tasks, IEnumerable<DaySummaryDto> DaySummaries);