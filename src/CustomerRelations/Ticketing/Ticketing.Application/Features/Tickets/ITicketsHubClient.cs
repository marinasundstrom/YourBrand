using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets;

public interface ITicketsHubClient
{
    Task Created(int todoId, string title);

    Task Updated(int todoId, string title);

    Task Deleted(int todoId, string title);

    Task TitleUpdated(int todoId, string title);

    Task DescriptionUpdated(int todoId, string? description);

    Task StatusUpdated(int todoId, TicketStatusDto status);

    Task EstimatedTimeUpdated(int todoId, TimeSpan? time);

    Task CompletedTimeUpdated(int todoId, TimeSpan? time);

    Task RemainingTimeUpdated(int todoId, TimeSpan? time);
}