namespace YourBrand.Catalog.Features.ProductManagement;

using YourBrand.Catalog.Features.ProductManagement.Options;

public record class CreateProductOptionData(string Name, string? Description, OptionType OptionType, OptionGroupDto? Group, bool? IsSelected, string? SKU, decimal? Price, string? GroupId, IEnumerable<CreateProductOptionValueData> Values,
    string? DefaultOptionValueId, int? MinNumericalValue, int? MaxNumericalValue, int? DefaultNumericalValue, int? TextValueMinLength, int? TextValueMaxLength, string? DefaultTextValue);