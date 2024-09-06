namespace YourBrand.Catalog.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Domain;
using YourBrand.Tenancy;

public class OptionValue : Entity<string>, IHasTenant, IHasOrganization
{
    protected OptionValue() { }

    public OptionValue(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int? Seq { get; set; }

    public ChoiceOption Option { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public string Name { get; set; } = null!;

    [Column("InventoryProductId")]
    public string? SKU { get; set; }

    public decimal? Price { get; set; }
}