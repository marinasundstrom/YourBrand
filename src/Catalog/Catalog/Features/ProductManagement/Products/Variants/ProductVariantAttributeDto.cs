namespace YourBrand.Catalog.Features.ProductManagement;

public record class ProductVariantAttributeDto(string Id, string Name, string? Value, string? ValueId, bool IsMainAttribute);