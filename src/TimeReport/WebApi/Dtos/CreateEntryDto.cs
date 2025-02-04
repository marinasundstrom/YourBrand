namespace YourBrand.TimeReport.Dtos;

public record class CreateEntryDto(string? Id, string? ProjectId, string? TaskId, DateTime Date, double? Hours, string? Description);