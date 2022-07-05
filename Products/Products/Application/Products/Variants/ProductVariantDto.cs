namespace YourBrand.Products.Application.Products.Variants;

public record class ProductVariantDto(string Id, string Name, string? Description, string? SKU, string? Image, decimal? Price, IEnumerable<ProductVariantDtoOption> Options);

