namespace YourBrand.Products.Domain.Entities;

public class VariantValue
{
    public int Id { get; set; }

    //public Product Product { get; set; } = null!;

    public ProductVariant Variant { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public OptionValue Value { get; set; } = null!;
}