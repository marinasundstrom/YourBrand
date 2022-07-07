namespace YourBrand.Catalog.Domain.Entities;

public class ProductOption
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public Product Product { get; set; } = null!;

    // This option is set for the product variant.

    public string? VariantId { get; set; } = null!;

    public ProductVariant? Variant { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public bool? IsSelected { get; set; }
}