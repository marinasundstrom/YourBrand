using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class Tag : Entity<int>, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public ProjectId ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public HashSet<TicketTag> TicketTags { get; } = new HashSet<TicketTag>();
}