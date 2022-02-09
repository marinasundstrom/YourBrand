namespace TimeReport.Dtos;

public record class UpdateEntryDto2(string? Id, string? ProjectId, string? ActivityId, DateTime? Date, double? Hours, string? Description);