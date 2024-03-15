namespace BlazorApp.Products;

public class AttributeGroupVM
{
    public string Id { get; set; }

    public string? Name { get; set; }

    public List<AttributeVM> Attributes { get; set; } = new List<AttributeVM>();
}