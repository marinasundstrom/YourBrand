using YourBrand.Identity;
using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketStatusUpdatedEventHandler(ITicketRepository ticketRepository, IEmailService emailService, ITicketNotificationService ticketNotificationService) : IDomainEventHandler<TicketStatusUpdated>
{
    public async Task Handle(TicketStatusUpdated notification, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.FindByIdAsync(notification.TicketId, cancellationToken);

        if (ticket is null)
            return;

        await ticketNotificationService.StatusUpdated(ticket.Id, ticket.Status.ToDto());

        if (ticket.AssigneeId is not null && ticket.LastModifiedById != ticket.AssigneeId)
        {
            await emailService.SendEmail(ticket.Assignee!.Email,
                $"Status of \"{ticket.Subject}\" [{ticket.Id}] changed to {notification.NewStatus}.",
                $"{ticket.LastModifiedBy!.Name} changed status of \"{ticket.Subject}\" [{ticket.Id}] from {notification.OldStatus} to {notification.NewStatus}.");
        }
    }
}