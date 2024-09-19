using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketEstimatedHoursUpdatedEventHandler(IApplicationDbContext context, ITicketRepository ticketRepository, IEmailService emailService, ITicketNotificationService ticketNotificationService, ISettableTenantContext tenantContext) : IDomainEventHandler<TicketEstimatedHoursUpdated>
{
    public async Task Handle(TicketEstimatedHoursUpdated notification, CancellationToken cancellationToken)
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
    }
}