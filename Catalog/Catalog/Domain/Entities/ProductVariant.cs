namespace YourBrand.Catalog.Domain.Entities;

public class ProductVariant
{
    public string Id { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? SKU { get; set; }

    public string? UPC { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public List<ProductVariantAttributeValue> AttributeValues { get; } = new List<ProductVariantAttributeValue>();

    public List<ProductVariantOption> ProductVariantOptions { get; } = new List<ProductVariantOption>();
}
