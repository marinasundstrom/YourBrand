using YourBrand.Catalog.Domain.Entities;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public sealed class Producer : AggregateRoot<int>, IHasTenant, IHasOrganization
{
    private Producer() : base(0) { }

    public Producer(string name, string handle) : base()
    {
        Name = name;
        Handle = handle;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;
}