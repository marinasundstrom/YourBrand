namespace YourBrand.Catalog.Domain.Entities;

public class ProductGroup
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ProductGroup? Parent { get; set; }

    public string? Image { get; set; }

    public List<ProductGroup> SubGroups { get; } = new List<ProductGroup>();

    public List<Product> Products { get; } = new List<Product>();
}
