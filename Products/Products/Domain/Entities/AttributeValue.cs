namespace YourBrand.Products.Domain.Entities;

public class AttributeValue
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public Attribute Attribute { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<VariantValue> VariantValues { get; } = new List<VariantValue>();
}
