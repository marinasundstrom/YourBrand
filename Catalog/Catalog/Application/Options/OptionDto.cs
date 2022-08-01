namespace YourBrand.Catalog.Application.Options;

public record class OptionDto(string Id, string Name, string? Description, OptionType OptionType, OptionGroupDto? Group, string? SKU, decimal? Price, bool IsSelected, IEnumerable<OptionValueDto> Values, OptionValueDto? DefaultValue, int? MinNumberValue, int? MaxNumberValue, int? DefaultNumberValue, int? StringValueMinLength, int? StringValueMaxLength, string? DefaultStringValue);

