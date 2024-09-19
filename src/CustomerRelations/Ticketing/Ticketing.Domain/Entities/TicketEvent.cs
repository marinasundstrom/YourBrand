using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.Events;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public sealed class TicketEvent : Entity<string>, IHasTenant, YourBrand.Domain.IHasOrganization
{
    public TicketEvent() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    //public TicketParticipantId OrganizationId { get; set; }

    public TicketId TicketId { get; set; }

    public string Event { get; set; }

    public TicketParticipantId ParticipantId { get; set; }

    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    public string Data { get; set; }
}
