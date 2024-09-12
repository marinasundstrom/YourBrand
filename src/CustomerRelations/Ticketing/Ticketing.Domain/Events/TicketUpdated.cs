using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUpdated(TenantId TenantId, string OrganizationId, TicketId TicketId) : DomainEvent;