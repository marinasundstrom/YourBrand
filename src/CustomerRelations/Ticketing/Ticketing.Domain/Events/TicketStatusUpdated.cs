using YourBrand.Ticketing.Domain.Entities;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;


namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketStatusUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketStatus2 NewStatus, TicketStatus2 OldStatus) : TicketDomainEvent(OrganizationId, TicketId);

public record TicketStatus2(int Id, string Name);