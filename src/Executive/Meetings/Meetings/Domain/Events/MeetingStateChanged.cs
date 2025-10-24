using YourBrand.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingStateChanged(
    TenantId TenantId,
    OrganizationId OrganizationId,
    MeetingId MeetingId,
    MeetingState State,
    string? AdjournmentMessage) : DomainEvent;
