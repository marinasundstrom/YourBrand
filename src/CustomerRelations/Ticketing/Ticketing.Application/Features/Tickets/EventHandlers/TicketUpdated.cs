using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketUpdatedEventHandler(ITicketRepository ticketRepository, ITicketNotificationService ticketNotificationService) : IDomainEventHandler<TicketUpdated>
{
    public async Task Handle(TicketUpdated notification, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.FindByIdAsync(notification.TicketId, cancellationToken);

        if (ticket is null)
            return;

        await ticketNotificationService.Updated(ticket.Id, ticket.Subject);
    }
}