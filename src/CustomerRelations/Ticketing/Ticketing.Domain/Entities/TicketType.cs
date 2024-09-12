using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Ticketing.Domain.Entities;

public sealed class TicketType : Entity<int>, IHasTenant, IHasOrganization
{
    protected TicketType() : base()
    {

    }

    public TicketType(int id, string name) : base()
    {
        Id = id;
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;
}