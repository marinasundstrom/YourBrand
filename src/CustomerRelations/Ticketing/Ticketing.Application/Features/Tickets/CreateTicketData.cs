using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets;

public record CreateTicketData(
    int ProjectId,
    string Title,
    string? Description,
    int Status,
    string? AssigneeId,
    double? EstimatedHours, double? RemainingHours,
    TicketPriorityDto? Priority, TicketImpactDto? Impact, TicketUrgencyDto? Urgency);
