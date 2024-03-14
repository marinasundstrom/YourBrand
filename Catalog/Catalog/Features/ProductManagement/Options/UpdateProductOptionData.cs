namespace YourBrand.Catalog.Features.ProductManagement;

public record class UpdateProductOptionData(string Name, string? Description, bool? IsSelected, string? SKU, decimal? Price, string? GroupId, IEnumerable<UpdateProductOptionValueData> Values,
    string? DefaultOptionValueId, int? MinNumericalValue, int? MaxNumericalValue, int? DefaultNumericalValue, int? TextValueMinLength, int? TextValueMaxLength, string? DefaultTextValue);