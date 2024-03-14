namespace YourBrand.Catalog.Features.ProductManagement;

public record class UpdateProductDetailsData(string Name, string? Description, string? Id, string? Image, decimal? Price, decimal? RegularPrice, long? GroupId);