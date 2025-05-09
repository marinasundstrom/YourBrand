using System.Text.Json.Serialization;

using YourBrand.Domain;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;


namespace YourBrand.Ticketing.Domain.Events;

//[JsonPolymorphic(TypeDiscriminatorPropertyName = "Event")]
[JsonDerivedType(typeof(TicketCreated), typeDiscriminator: nameof(TicketCreated))]
[JsonDerivedType(typeof(TicketDeleted), typeDiscriminator: nameof(TicketDeleted))]
[JsonDerivedType(typeof(TicketProjectUpdated), typeDiscriminator: nameof(TicketProjectUpdated))]
[JsonDerivedType(typeof(TicketAssigneeUpdated), typeDiscriminator: nameof(TicketAssigneeUpdated))]
[JsonDerivedType(typeof(TicketDescriptionUpdated), typeDiscriminator: nameof(TicketDescriptionUpdated))]
[JsonDerivedType(typeof(TicketEstimatedTimeUpdated), typeDiscriminator: nameof(TicketEstimatedTimeUpdated))]
[JsonDerivedType(typeof(TicketRemainingTimeUpdated), typeDiscriminator: nameof(TicketRemainingTimeUpdated))]
[JsonDerivedType(typeof(TicketCompletedHoursUpdated), typeDiscriminator: nameof(TicketCompletedHoursUpdated))]
[JsonDerivedType(typeof(TicketStatusUpdated), typeDiscriminator: nameof(TicketStatusUpdated))]
[JsonDerivedType(typeof(TicketSubjectUpdated), typeDiscriminator: nameof(TicketSubjectUpdated))]
[JsonDerivedType(typeof(TicketPriorityUpdated), typeDiscriminator: nameof(TicketPriorityUpdated))]
[JsonDerivedType(typeof(TicketUrgencyUpdated), typeDiscriminator: nameof(TicketUrgencyUpdated))]
[JsonDerivedType(typeof(TicketImpactUpdated), typeDiscriminator: nameof(TicketImpactUpdated))]
[JsonDerivedType(typeof(TicketCommentAdded), typeDiscriminator: nameof(TicketCommentAdded))]
public record TicketDomainEvent(OrganizationId OrganizationId, TicketId TicketId) : DomainEvent, IHasOrganization2, IHasTicket;

public interface IHasTicket
{
    TicketId TicketId { get; }
}

public interface IHasOrganization2
{
    OrganizationId OrganizationId { get; }
}