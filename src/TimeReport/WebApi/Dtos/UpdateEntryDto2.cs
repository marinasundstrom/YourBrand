namespace YourBrand.TimeReport.Dtos;

public record class UpdateEntryDto2(string? Id, string? ProjectId, string? TaskId, DateTime? Date, double? Hours, string? Description);