namespace YourBrand.Catalog.Domain.Entities;

public class ProductAttribute
{
    public int Id { get; set; }

    public string ProductId { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public string AttributeId { get; set; } = null!;

    public Entities.Attribute Attribute { get; set; } = null!;

}
