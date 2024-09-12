using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDeleted(TenantId TenantId, string OrganizationId, TicketId TicketId, string Title) : DomainEvent;