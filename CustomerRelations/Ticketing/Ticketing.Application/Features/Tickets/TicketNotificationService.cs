using Microsoft.AspNetCore.SignalR;
using YourBrand.Ticketing.Application.Services;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets;

public class TicketNotificationService : ITicketNotificationService
{
    private readonly IHubContext<TicketsHub, ITicketsHubClient> hubsContext;

    public TicketNotificationService(IHubContext<TicketsHub, ITicketsHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }

    public async Task Created(int ticketId, string title)
    {
        await hubsContext.Clients.All.Created(ticketId, title);
    }

    public async Task Updated(int ticketId, string title)
    {
        await hubsContext.Clients.All.Updated(ticketId, title);
    }

    public async Task Deleted(int ticketId, string title)
    {
        await hubsContext.Clients.All.Deleted(ticketId, title);
    }

    public async Task DescriptionUpdated(int ticketId, string? description)
    {
        await hubsContext.Clients.All.DescriptionUpdated(ticketId, description);
    }

    public async Task StatusUpdated(int ticketId, TicketStatusDto status)
    {
        await hubsContext.Clients.All.StatusUpdated(ticketId, status);
    }

    public async Task TitleUpdated(int ticketId, string title)
    {
        await hubsContext.Clients.All.TitleUpdated(ticketId, title);
    }

    public async Task EstimatedHoursUpdated(int ticketId, double? hours)
    {
        await hubsContext.Clients.All.EstimatedHoursUpdated(ticketId, hours);
    }

    public async Task RemainingHoursUpdated(int ticketId, double? hours)
    {
        await hubsContext.Clients.All.RemainingHoursUpdated(ticketId, hours);
    }
}