namespace YourBrand.Catalog.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Catalog.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

public abstract class Option : Entity<string>, IHasTenant, IHasOrganization
{
    protected Option() { }

    public Option(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public OptionGroup? Group { get; set; }

    public ProductCategory? ProductCategory { get; set; }

    public OptionType OptionType { get; protected set; }

    public bool IsRequired { get; set; }

    public List<Product> Products { get; } = new List<Product>();

    public List<ProductOption> ProductOption { get; } = new List<ProductOption>();

    public List<ProductVariantOption> ProductVariantOptions { get; } = new List<ProductVariantOption>();
}

public sealed class SelectableOption : Option
{
    private SelectableOption() { }

    public SelectableOption(string name)
        : base(name)
    {
        OptionType = OptionType.YesOrNo;
    }

    public bool IsSelected { get; set; }

    [Column("InventoryProductId")]
    public string? SKU { get; set; }

    public decimal? Price { get; set; }
}

public sealed class ChoiceOption : Option
{
    readonly List<OptionValue> _values = new List<OptionValue>();

    private ChoiceOption() { }

    public ChoiceOption(string name)
        : base(name)
    {
        OptionType = OptionType.Choice;
    }

    public IReadOnlyCollection<OptionValue> Values => _values;

    public void AddValue(OptionValue optionValue)
    {
        _values.Add(optionValue);
        optionValue.OrganizationId = OrganizationId;
    }

    public void RemoveValue(OptionValue optionValue)
    {
        _values.Remove(optionValue);
    }

    [ForeignKey(nameof(DefaultValue))]
    public string? DefaultValueId { get; set; }

    public OptionValue? DefaultValue { get; set; }
}

public sealed class NumericalValueOption : Option
{
    private NumericalValueOption() { }

    public NumericalValueOption(string name)
        : base(name)
    {
        OptionType = OptionType.NumericalValue;
    }

    public int? MinNumericalValue { get; set; }

    public int? MaxNumericalValue { get; set; }

    public int? DefaultNumericalValue { get; set; }

    public decimal? Price { get; set; }
}

public sealed class TextValueOption : Option
{
    private TextValueOption() { }

    public TextValueOption(string name)
        : base(name)
    {
        OptionType = OptionType.TextValue;
    }

    public int? TextValueMinLength { get; set; }

    public int? TextValueMaxLength { get; set; }

    public string? DefaultTextValue { get; set; }
}