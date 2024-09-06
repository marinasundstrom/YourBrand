using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public sealed class Brand : AggregateRoot<int>, IHasTenant, IHasOrganization
{
    readonly HashSet<Brand> _subBrands = new HashSet<Brand>();

    private Brand() : base(0) { }

    public Brand(string name, string handle) : base()
    {
        Name = name;
        Handle = handle;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public Brand? Parent { get; set; }

    public int? ParentId { get; set; }

    public IReadOnlyCollection<Brand> SubBrands => _subBrands;
}

/*
public sealed class Merchant : AggregateRoot<string>
{
    private Merchant() { }

    public Merchant(string id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; set; } = null!;
}
*/