namespace YourBrand.Catalog.Domain.Entities;

public class ProductVariantOption
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public Product Product{ get; set; } = null!;

    public string ProductVariantId { get; set; } = null!;

    public ProductVariant ProductVariant { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public bool? IsSelected { get; set; }

    // Add fields for default values
}
