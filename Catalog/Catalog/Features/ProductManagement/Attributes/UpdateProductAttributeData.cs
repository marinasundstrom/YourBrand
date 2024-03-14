namespace YourBrand.Catalog.Features.ProductManagement;

public record class UpdateProductAttributeData(string Name, string? Description, bool ForVariant, bool IsMainAttribute, string? GroupId, IEnumerable<UpdateProductAttributeValueData> Values);