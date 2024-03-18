using System;
using MediatR;
using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Services;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketCreatedEventHandler : IDomainEventHandler<TicketCreated>
{
    private readonly ITicketRepository ticketRepository;
    private readonly ITicketNotificationService ticketNotificationService;

    public TicketCreatedEventHandler(ITicketRepository ticketRepository, ITicketNotificationService ticketNotificationService)
    {
        this.ticketRepository = ticketRepository;
        this.ticketNotificationService = ticketNotificationService;
    }

    public async Task Handle(TicketCreated notification, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.FindByIdAsync(notification.TicketId, cancellationToken);

        if (ticket is null)
            return;

        await ticketNotificationService.Created(ticket.Id, ticket.Subject);
    }
}

