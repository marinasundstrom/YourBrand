namespace TimeReport.Application.TimeSheets;

public record class TimeSheetEntryDto(string Id, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);