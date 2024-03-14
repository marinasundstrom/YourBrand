namespace YourBrand.Catalog.Features.ProductManagement;

public record class UpdateProductCategoryData(string Name, string Handle, string? Description, long? ParentGroupId, bool AllowItems);