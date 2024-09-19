using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketStatusUpdatedEventHandler(IApplicationDbContext context, ITicketRepository ticketRepository, IEmailService emailService, ITicketNotificationService ticketNotificationService, ISettableTenantContext tenantContext) : IDomainEventHandler<TicketStatusUpdated>
{
    public async Task Handle(TicketStatusUpdated notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var ticket = await ticketRepository.FindByIdAsync(notification.TicketId, cancellationToken);

        if (ticket is null)
            return;

        await ticketNotificationService.StatusUpdated(ticket.Id, ticket.Status.ToDto());

        var ev = new TicketEvent();
        ev.OrganizationId = notification.OrganizationId;
        ev.TicketId = notification.TicketId;
        ev.Event = notification.GetType().Name.Replace("Ticket", string.Empty);
        ev.ParticipantId = ticket.LastModifiedById.GetValueOrDefault();
        ev.OccurredAt = notification.Timestamp;
        ev.Data = System.Text.Json.JsonSerializer.Serialize<TicketDomainEvent>(notification);

        context.TicketEvents.Add(ev);

        await context.SaveChangesAsync(cancellationToken);

        if (ticket.AssigneeId is not null && ticket.LastModifiedById != ticket.AssigneeId)
        {
            /*
            await emailService.SendEmail(ticket.Assignee!.Email,
                $"Status of \"{ticket.Subject}\" [{ticket.Id}] changed to {notification.NewStatus}.",
                $"{ticket.LastModifiedBy!.Name} changed status of \"{ticket.Subject}\" [{ticket.Id}] from {notification.OldStatus} to {notification.NewStatus}.");
            */
        }
    }
}