namespace YourBrand.Catalog.Application.Options;

public record class OptionGroupDto(string Id, string Name, string? Description, int? Seq, int? Min, int? Max);

