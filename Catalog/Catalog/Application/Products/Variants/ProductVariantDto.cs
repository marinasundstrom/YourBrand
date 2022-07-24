namespace YourBrand.Catalog.Application.Products.Variants;

public record class ProductVariantDto(string Id, string Name, string? Description, string? SKU, string? Image, decimal? Price, IEnumerable<ProductVariantAttributeDto> Attributes);

