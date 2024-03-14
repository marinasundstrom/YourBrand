namespace YourBrand.Catalog.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

public class OptionValue : Entity<string>
{
    protected OptionValue() { }

    public OptionValue(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public int? Seq { get; set; }

    public ChoiceOption Option { get; set; } = null!;

    public string Name { get; set; } = null!;

    [Column("InventoryProductId")]
    public string? SKU { get; set; }

    public decimal? Price { get; set; }
}