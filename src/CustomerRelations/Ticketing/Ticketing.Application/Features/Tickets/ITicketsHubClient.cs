using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets;

public interface ITicketsHubClient
{
    Task Created(int ticketId, string title);

    Task Updated(int ticketId, string title);

    Task Deleted(int ticketId, string title);

    Task TitleUpdated(int ticketId, string title);

    Task DescriptionUpdated(int ticketId, string? description);

    Task StatusUpdated(int ticketId, TicketStatusDto status);

    Task EstimatedTimeUpdated(int ticketId, TimeSpan? time);

    Task CompletedTimeUpdated(int ticketId, TimeSpan? time);

    Task RemainingTimeUpdated(int ticketId, TimeSpan? time);

    Task AssigneeUpdated(int ticketId, string? participantId, string? name);
}