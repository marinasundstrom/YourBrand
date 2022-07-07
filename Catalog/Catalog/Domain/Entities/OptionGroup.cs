namespace YourBrand.Catalog.Domain.Entities;

public class OptionGroup
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Product? Product { get; set; }

    public List<Option> Options { get; } = new List<Option>();

    public int? Min { get; set; }

    public int? Max { get; set; }
}
