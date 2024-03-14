namespace YourBrand.Catalog.Features.ProductManagement;

public record class UpdateProductVariantData(string Name, string? Description, string Id, decimal Price, decimal? RegularPrice, IEnumerable<UpdateProductVariantAttributeData> Attributes);