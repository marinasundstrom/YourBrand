namespace YourBrand.Catalog.Application.Options;

public record class OptionValueDto(string Id, string Name, string? SKU, decimal? Price, int? Seq);

