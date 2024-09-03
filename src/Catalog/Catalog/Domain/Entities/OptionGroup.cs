using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class OptionGroup : AggregateRoot<string>, IHasTenant, IHasOrganization
{
    protected OptionGroup() { }

    public OptionGroup(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int? Seq { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Product? Product { get; set; }

    public List<Option> Options { get; } = new List<Option>();

    public int? Min { get; set; }

    public int? Max { get; set; }
}