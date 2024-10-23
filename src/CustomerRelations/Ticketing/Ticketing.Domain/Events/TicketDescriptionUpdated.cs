using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDescriptionUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string? NewDescription, string? OldDescription) : TicketDomainEvent(OrganizationId, TicketId);