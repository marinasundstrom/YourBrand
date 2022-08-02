namespace YourBrand.Catalog.Domain.Entities;

public class OptionValue
{
    public string Id { get; set; } = null!;

    public int? Seq { get; set; }

    public Option Option { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? SKU { get; set; }

    public decimal? Price { get; set; }
}
