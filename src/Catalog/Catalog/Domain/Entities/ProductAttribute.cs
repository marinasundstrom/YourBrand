namespace YourBrand.Catalog.Domain.Entities;

public class ProductAttribute : Entity<int>
{
    public long ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string AttributeId { get; set; } = null!;

    public Entities.Attribute Attribute { get; set; } = null!;

    public bool ForVariant { get; set; }

    public bool IsMainAttribute { get; set; }

    public AttributeValue? Value { get; set; } = null!;
}