namespace TimeReport.Dtos;

public record class CreateEntryDto(string? Id, string? ProjectId, string? ActivityId, DateTime Date, double? Hours, string? Description);