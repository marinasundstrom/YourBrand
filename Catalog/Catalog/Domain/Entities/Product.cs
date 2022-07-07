using YourBrand.Catalog.Domain.Enums;

namespace YourBrand.Catalog.Domain.Entities;

public class Product
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public ProductGroup? Group { get; set; }

    public string? Description { get; set; } = null!;

    public string? SKU { get; set; }

    public string? UPC { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public bool HasVariants { get; set; } = false;

    public bool? AllCustom { get; set; }

    public List<Entities.Attribute> Attributes { get; } = new List<Entities.Attribute>();

    public List<ProductAttribute> ProductAttributes { get; } = new List<ProductAttribute>();

    public List<AttributeGroup> AttributeGroups { get; } = new List<AttributeGroup>();

    public List<ProductVariant> Variants { get; } = new List<ProductVariant>();

    public List<Option> Options { get; } = new List<Option>();

    public List<ProductOption> ProductOptions { get; } = new List<ProductOption>();

    public List<OptionGroup> OptionGroups { get; } = new List<OptionGroup>();

    public ProductVisibility Visibility { get; set; }
}
