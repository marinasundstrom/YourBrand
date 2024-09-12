using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketParticipant : Entity<int>, IAuditable, IHasTenant, IHasOrganization
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int TicketId { get; set; }

    public string? Name { get; set; }

    public UserId? User { get; set; }

    public string? Email { get; set; }
}