namespace YourBrand.Catalog.Features.ProductManagement;

public record class CreateProductVariantData(string Name, string Handle, string? Description, string? Id, decimal Price, decimal? RegularPrice, IEnumerable<CreateProductVariantAttributeData> Attributes);