using YourBrand.Domain;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Meetings.Domain.Events;

public sealed record MeetingAgendaItemStateChanged(
    TenantId TenantId,
    OrganizationId OrganizationId,
    MeetingId MeetingId,
    AgendaItemId AgendaItemId,
    AgendaItemState State,
    AgendaItemPhase Phase) : DomainEvent;
