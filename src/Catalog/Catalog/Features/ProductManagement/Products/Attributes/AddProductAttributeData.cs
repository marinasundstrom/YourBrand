namespace YourBrand.Catalog.Features.ProductManagement;

public record class AddProductAttributeData(string Name, string? Description, bool ForVariant, bool IsMainAttribute, string? GroupId, IEnumerable<CreateProductAttributeValueData> Values);