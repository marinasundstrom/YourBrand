using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingUpdated(TenantId TenantId, OrganizationId OrganizationId, MeetingId MeetingId);