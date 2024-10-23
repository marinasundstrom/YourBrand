using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

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

    public ProjectId ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}