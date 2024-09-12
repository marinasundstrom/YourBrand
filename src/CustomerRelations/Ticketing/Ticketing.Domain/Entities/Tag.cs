using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Ticketing.Domain.Entities;

public class Tag : Entity<int>, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public HashSet<TicketTag> TicketTags { get; } = new HashSet<TicketTag>();
}