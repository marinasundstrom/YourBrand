namespace YourBrand.Catalog.Domain.Entities;

public class Attribute
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public AttributeGroup? Group { get; set; }

    public bool ForVariant { get; set; }

    public List<Product> Products { get; } = new List<Product>();

    public List<AttributeValue> Values { get; } = new List<AttributeValue>();
}
