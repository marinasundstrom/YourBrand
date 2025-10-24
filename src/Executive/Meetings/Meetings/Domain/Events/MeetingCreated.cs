using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingCreated(TenantId TenantId, OrganizationId OrganizationId, MeetingId MeetingId) : DomainEvent;
