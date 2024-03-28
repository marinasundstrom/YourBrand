namespace BlazorApp.Products;

public class AttributeVM
{
    public string Id { get; set; }

    public string Name { get; set; }

    public AttributeGroupVM Group { get; set; }

    public List<AttributeValueVM> Values { get; set; } = new List<AttributeValueVM>();

    public bool ForVariant { get; set; }

    public bool IsMainAttribute { get; set; }

    public string? SelectedValueId { get; set; }
}