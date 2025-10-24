using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingScheduledAtChanged(
    TenantId TenantId,
    OrganizationId OrganizationId,
    MeetingId MeetingId,
    DateTimeOffset ScheduledAt) : DomainEvent;
