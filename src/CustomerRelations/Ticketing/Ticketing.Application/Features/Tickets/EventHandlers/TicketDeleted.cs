using YourBrand.Domain;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketDeletedEventHandler(ITicketRepository ticketRepository, ITicketNotificationService ticketNotificationService) : IDomainEventHandler<TicketDeleted>
{
    public async Task Handle(TicketDeleted notification, CancellationToken cancellationToken)
    {
        await ticketNotificationService.Deleted(notification.TicketId, notification.Title);
    }
}