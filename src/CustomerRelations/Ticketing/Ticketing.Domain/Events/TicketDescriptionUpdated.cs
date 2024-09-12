using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketTextUpdated(TenantId TenantId, string OrganizationId, TicketId TicketId, string? Description) : DomainEvent;