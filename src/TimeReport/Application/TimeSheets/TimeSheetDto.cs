using YourBrand.TimeReport.Application.Users;

namespace YourBrand.TimeReport.Application.TimeSheets;

public record class TimeSheetDto(string Id, int Year, int Week, DateTime From, DateTime To, TimeSheetStatusDto Status, UserDto User, IEnumerable<TimeSheetProjectDto> Projects, IEnumerable<DaySummaryDto> DaySummaries, IEnumerable<ReportingPeriodDto> Months);