using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketCreatedEventHandler(ITicketRepository ticketRepository, ITicketNotificationService ticketNotificationService) : IDomainEventHandler<TicketCreated>
{
    public async Task Handle(TicketCreated notification, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.FindByIdAsync(notification.TicketId, cancellationToken);

        if (ticket is null)
            return;

        await ticketNotificationService.Created(ticket.Id, ticket.Subject);
    }
}