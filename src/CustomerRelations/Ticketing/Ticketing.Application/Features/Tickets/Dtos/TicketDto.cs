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

    // Planning
    TicketPriorityDto? Priority,
    TicketUrgencyDto? Urgency,
    TicketImpactDto? Impact,

    //s Effort
    TimeSpan? EstimatedTime,
    TimeSpan? CompletedTime,
    TimeSpan? RemainingTime,

    // Schedule
    DateTimeOffset? PlannedStartDate,
    DateTimeOffset? StartDeadline,
    DateTimeOffset? ExpectedEndDate,
    DateTimeOffset? DueDate,
    DateTimeOffset? ActualStartDate,
    DateTimeOffset? ActualEndDate,

    IEnumerable<TagDto> Tags,
    IEnumerable<AttachmentDto> Attachments,
    
    DateTimeOffset Created,
    TicketParticipantDto? CreatedBy,
    DateTimeOffset? LastModified,
    TicketParticipantDto? LastModifiedBy);