using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class Attachment : Entity<int>, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public TicketId TicketId { get; set; }

    public string Name { get; set; } = null!;
}