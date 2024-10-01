using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

using System.Text.Json.Serialization;
using YourBrand.Ticketing.Application.Features.Projects;

namespace YourBrand.Ticketing.Application;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "event")]
[JsonDerivedType(typeof(TicketCreatedDto), "Created")]
[JsonDerivedType(typeof(TicketProjectUpdatedDto), "ProjectUpdated")]
[JsonDerivedType(typeof(TicketAssigneeUpdatedDto), "AssigneeUpdated")]
[JsonDerivedType(typeof(TicketDescriptionUpdatedDto), "DescriptionUpdated")]
[JsonDerivedType(typeof(TicketEstimatedHoursUpdatedDto), "EstimatedHoursUpdated")]
[JsonDerivedType(typeof(TicketRemainingHoursUpdatedDto), "RemainingHoursUpdated")]
[JsonDerivedType(typeof(TicketStatusUpdatedDto), "StatusUpdated")]
[JsonDerivedType(typeof(TicketSubjectUpdatedDto), "SubjectUpdated")]
[JsonDerivedType(typeof(TicketPriorityUpdatedDto), "PriorityUpdated")]
[JsonDerivedType(typeof(TicketUrgencyUpdatedDto), "UrgencyUpdated")]
[JsonDerivedType(typeof(TicketImpactUpdatedDto), "ImpactUpdated")]
[JsonDerivedType(typeof(TicketCommentAddedDto), "CommentAdded")]
public abstract record TicketEventDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, TicketParticipantDto Participant) 
{
    public string Event => GetType().Name.Replace("Ticket", string.Empty).Replace("Dto", string.Empty);
}

public sealed record TicketCreatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketProjectUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, ProjectDto? Project, ProjectDto? OldProject, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketAssigneeUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketParticipantDto? AssignedParticipant, TicketParticipantDto? OldAssignedParticipant, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketDescriptionUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string? NewDescription, string? OldDescription, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketEstimatedHoursUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, double? NewHours, double? OldHours, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketRemainingHoursUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, double? NewHours, double? OldHours, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketStatusUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketStatusDto? NewStatus, TicketStatusDto? OldStatus, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketSubjectUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, string? NewSubject, string? OldSubject, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketPriorityUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketPriorityDto? NewPriority, TicketPriorityDto? OldPriority, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketUrgencyUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketUrgencyDto? NewUrgency, TicketUrgencyDto? OldUrgency, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketImpactUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TicketImpactDto? NewImpact, TicketImpactDto? OldImpact, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketCommentAddedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, int CommentId, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);


public static partial class Mappings
{
    /*
    public static TicketEventDto ToDto(this TicketDomainEvent @event, TicketEvent ev) 
    {
        return @event switch {
            TicketCreated e => new TicketCreatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, ev.Participant),
            TicketAssigneeUpdated e => new TicketAssigneeUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewAssignedParticipant, e.OldAssignedParticipant, ev.Participant!.ToDto()),
            TicketDescriptionUpdated e => new TicketDescriptionUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewDescription, e.OldDescription, ev.Participant!.ToDto()),
            TicketEstimatedHoursUpdated e => new TicketEstimatedHoursUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewHours, e.OldHours, ev.Participant!.ToDto()),
            TicketRemainingHoursUpdated e => new TicketRemainingHoursUpdatedDto(ev.OccurredAt, ev.TenantId, e.OrganizationId, e.TicketId, e.NewHours, e.OldHours, ev.Participant!.ToDto()),
            TicketStatusUpdated e => new TicketStatusUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewStatus.Id, e.OldStatus.Id, ev.Participant!.ToDto()),
            TicketSubjectUpdated e => new TicketSubjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewSubject, e.OldSubject, ev.Participant!.ToDto()),
            _ => throw new Exception()
        };
    }
    */
}