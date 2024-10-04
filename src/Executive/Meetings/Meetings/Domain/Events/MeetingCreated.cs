using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingCreated(TenantId TenantId, OrganizationId OrganizationId, MeetingId MeetingId);