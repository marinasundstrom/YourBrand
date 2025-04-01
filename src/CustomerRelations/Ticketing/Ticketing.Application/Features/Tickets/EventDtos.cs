using System.Text.Json.Serialization;

using YourBrand.Ticketing.Application.Features.Projects;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "event")]
[JsonDerivedType(typeof(TicketCreatedDto), "Created")]
[JsonDerivedType(typeof(TicketProjectUpdatedDto), "ProjectUpdated")]
[JsonDerivedType(typeof(TicketAssigneeUpdatedDto), "AssigneeUpdated")]
[JsonDerivedType(typeof(TicketDescriptionUpdatedDto), "DescriptionUpdated")]
[JsonDerivedType(typeof(TicketEstimatedTimeUpdatedDto), "EstimatedTimeUpdated")]
[JsonDerivedType(typeof(TicketRemainingTimeUpdatedDto), "RemainingTimeUpdated")]
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

public sealed record TicketEstimatedTimeUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TimeSpan? NewTime, TimeSpan? OldTime, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketRemainingTimeUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TimeSpan? NewTime, TimeSpan? OldTime, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

public sealed record TicketCompletedHoursUpdatedDto(DateTimeOffset OccurredAt, string TenantId, string OrganizationId, int TicketId, TimeSpan? NewTime, TimeSpan? OldTime, TicketParticipantDto Participant) : TicketEventDto(OccurredAt, TenantId, OrganizationId, Participant);

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
            TicketEstimatedTimeUpdated e => new TicketEstimatedTimeUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewTime, e.OldTime, ev.Participant!.ToDto()),
            TicketRemainingTimeUpdated e => new TicketRemainingTimeUpdatedDto(ev.OccurredAt, ev.TenantId, e.OrganizationId, e.TicketId, e.NewTime, e.OldTime, ev.Participant!.ToDto()),
            TicketStatusUpdated e => new TicketStatusUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewStatus.Id, e.OldStatus.Id, ev.Participant!.ToDto()),
            TicketSubjectUpdated e => new TicketSubjectUpdatedDto(ev.OccurredAt, ev.TenantId, ev.OrganizationId, e.TicketId, e.NewSubject, e.OldSubject, ev.Participant!.ToDto()),
            _ => throw new Exception()
        };
    }
    */
}