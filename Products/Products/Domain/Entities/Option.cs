namespace YourBrand.Products.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Products.Domain.Enums;

public class Option
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public OptionGroup? Group { get; set; }

    public OptionType OptionType { get; set; } = OptionType.Multiple;

    public bool IsRequired { get; set; }

    public bool IsSelected { get; set; }

    public string? SKU { get; set; }

    public decimal? Price { get; set; }

    public List<OptionValue> Values { get; } = new List<OptionValue>();

    public List<Product> Products { get; } = new List<Product>();

    [ForeignKey(nameof(DefaultValue))]
    public string? DefaultValueId { get; set; }

    public OptionValue? DefaultValue { get; set; }
}
