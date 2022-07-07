using YourBrand.Catalog.Application.Products.Groups;

namespace YourBrand.Catalog.Application.Products;

public record class ProductDto(string Id, string Name, string? Description, ProductGroupDto? Group, string? SKU, string? Image, decimal? Price, bool HasVariants, ProductVisibility? Visibility);

