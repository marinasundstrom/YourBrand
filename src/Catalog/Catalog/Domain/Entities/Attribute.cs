using System.Collections.ObjectModel;

using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class Attribute : Entity<string>, IHasTenant, IHasOrganization
{
    HashSet<AttributeValue> _values = new HashSet<AttributeValue>();

    protected Attribute() { }

    public Attribute(string name)
        : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public AttributeGroup? Group { get; set; }

    public ProductCategory? ProductCategory { get; set; }

    //public List<Product> Products { get; } = new List<Product>();

    public List<ProductAttribute> ProductAttributes { get; } = new List<ProductAttribute>();

    public IReadOnlyCollection<AttributeValue> Values => _values;

    public void AddValue(AttributeValue attributeValue) 
    {
        _values.Add(attributeValue);
        attributeValue.OrganizationId = OrganizationId;
    }

    public void RemoveValue(AttributeValue attributeValue)
    {
        _values.Remove(attributeValue);
    }
}