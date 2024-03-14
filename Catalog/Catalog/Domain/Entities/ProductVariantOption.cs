namespace YourBrand.Catalog.Domain.Entities;

public class ProductVariantOption : Entity<int>
{
    public long ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string ProductVariantId { get; set; } = null!;

    public Product ProductVariant { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public bool? IsSelected { get; set; }

    // Add fields for default values
}