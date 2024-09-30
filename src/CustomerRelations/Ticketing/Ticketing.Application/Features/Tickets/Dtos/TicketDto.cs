using YourBrand.Ticketing.Application.Features.Projects;

namespace YourBrand.Ticketing.Application.Features.Tickets.Dtos;
public sealed record TicketDto(
    int Id,
    ProjectDto Project,
    string Subject,
    string? Description,
    TicketStatusDto Status,
    TicketParticipantDto? Assignee,
    DateTime? LastMessage,
    string? Text,
    TicketTypeDto Type,
    TicketPriorityDto? Priority,
    TicketUrgencyDto? Urgency,
    TicketImpactDto? Impact,
    double? EstimatedHours,
    double? RemainingHours,
    IEnumerable<TagDto> Tags,
    IEnumerable<AttachmentDto> Attachments,
    DateTimeOffset Created,
    TicketParticipantDto? CreatedBy,
    DateTimeOffset? LastModified,
    TicketParticipantDto? LastModifiedBy);