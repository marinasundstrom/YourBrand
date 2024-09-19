using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.Events;

using System.Runtime.Serialization;
using Newtonsoft.Json;

using NJsonSchema.Converters;

namespace YourBrand.Ticketing.Application;

[JsonConverter(typeof(JsonInheritanceConverter<TicketEventDto>), "type")]
[KnownType(typeof(TicketCreatedDto))]
[KnownType(typeof(TicketAssigneeUpdatedDto))]
[KnownType(typeof(TicketDescriptionUpdatedDto))]
[KnownType(typeof(TicketEstimatedHoursUpdatedDto))]
[KnownType(typeof(TicketRemainingHoursUpdatedDto))]
[KnownType(typeof(TicketStatusUpdatedDto))]
[KnownType(typeof(TicketSubjectUpdatedDto))]
public abstract record TicketEventDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, string ParticipantId) 
{
    public string Type => GetType().Name;
}

public sealed record TicketCreatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketAssigneeUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string? AssignedParticipantId, string? OldAssignedParticipantId, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketDescriptionUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string? NewDescription, string? OldDescription, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketEstimatedHoursUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, double? NewHours, double? OldHours, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketRemainingHoursUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, double? NewHours, double? OldHours, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketStatusUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, int? NewStatus, int? OldStatus, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public sealed record TicketSubjectUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string? NewSubject, string? OldSubject, string ParticipantId) : TicketEventDto(OccurredAt, TenantId, OrganizationId, ParticipantId);

public static partial class Mappings
{
    public static TicketEventDto ToDto(this TicketDomainEvent @event, TicketEvent ev) 
    {
        return @event switch {
            TicketCreated e => new TicketCreatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, ev.ParticipantId),
            TicketAssigneeUpdated e => new TicketAssigneeUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewAssignedParticipantId, e.OldAssignedParticipantId, ev.ParticipantId),
            TicketDescriptionUpdated e => new TicketDescriptionUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewDescription, e.OldDescription, ev.ParticipantId),
            TicketEstimatedHoursUpdated e => new TicketEstimatedHoursUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewHours, e.OldHours, ev.ParticipantId),
            TicketRemainingHoursUpdated e => new TicketRemainingHoursUpdatedDto(ev.OccurredAt, ev.TenantId, e.OrganizationId, e.TicketId, e.NewHours, e.OldHours, ev.ParticipantId),
            TicketStatusUpdated e => new TicketStatusUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewStatus.Id, e.OldStatus.Id, ev.ParticipantId),
            TicketSubjectUpdated e => new TicketSubjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewSubject, e.OldSubject, ev.ParticipantId),
            _ => throw new Exception()
        };
    }
}