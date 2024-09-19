using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDescriptionUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string? NewDescription, string? OldDescription) : TicketDomainEvent(OrganizationId, TicketId);