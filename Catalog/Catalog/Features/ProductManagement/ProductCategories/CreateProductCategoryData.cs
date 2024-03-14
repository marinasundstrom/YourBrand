namespace YourBrand.Catalog.Features.ProductManagement;

public record class CreateProductCategoryData(string Name, string Handle, string? Description, long? ParentGroupId, bool AllowItems);