namespace YourBrand.Ticketing.Application.Features.Tickets.Dtos;

using YourBrand.Ticketing.Application.Features.Users;

public sealed record TicketDto(
    int Id, 
    string Requester,
    string Subject, 
    string? Description, 
    TicketStatusDto Status, 
    UserDto? Assignee,
    DateTime? LastMessage,
    string? Text,
    TicketTypeDto Type,
    TicketPriorityDto Priority,
    TicketSeverityDto Severity,
    TicketImpactDto Impact,
    double? EstimatedHours, 
    double? RemainingHours,
    IEnumerable<TagDto> Tags,
    IEnumerable<AttachmentDto> Attachments,
    DateTimeOffset Created, 
    UserDto? CreatedBy, 
    DateTimeOffset? LastModified, 
    UserDto? LastModifiedBy);