namespace YourBrand.Catalog.Domain.Entities;

public class ProductVariantAttributeValue
{
    public int Id { get; set; }

    //public Product Product { get; set; } = null!;

    public ProductVariant Variant { get; set; } = null!;

    public Entities.Attribute Attribute { get; set; } = null!;

    public AttributeValue Value { get; set; } = null!;
}