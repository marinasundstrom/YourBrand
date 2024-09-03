using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class AttributeGroup : Entity<string>, IHasTenant, IHasOrganization
{
    protected AttributeGroup() { }

    public AttributeGroup(string name)
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

    public List<Entities.Attribute> Attributes { get; } = new List<Entities.Attribute>();
}