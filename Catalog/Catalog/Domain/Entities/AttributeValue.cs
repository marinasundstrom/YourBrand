namespace YourBrand.Catalog.Domain.Entities;

public class AttributeValue
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public Attribute Attribute { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<ProductVariantAttributeValue> ProductVariantValues { get; } = new List<ProductVariantAttributeValue>();
}
