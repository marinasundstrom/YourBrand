namespace YourBrand.Catalog.Domain.Entities;

public class AttributeGroup
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Product? Product { get; set; }

    public List<Entities.Attribute> Attributes { get; } = new List<Entities.Attribute>();
}
