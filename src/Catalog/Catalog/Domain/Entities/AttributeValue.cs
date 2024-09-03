using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class AttributeValue : Entity<string>, IHasTenant, IHasOrganization
{
    protected AttributeValue() { }

    public AttributeValue(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int? Seq { get; set; }

    public Attribute Attribute { get; set; } = null!;

    public string Name { get; set; } = null!;

    public List<ProductAttribute> ProductAttributes { get; } = new List<ProductAttribute>();
}