using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class TicketStatus : Entity<int>, IHasTenant, IHasOrganization
{
    public TicketStatus()
    {

    }

    public TicketStatus(int id) 
    {
        Id = id;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public ProjectId ProjectId { get; set; }

    public string Name { get; set; } = null!;
}