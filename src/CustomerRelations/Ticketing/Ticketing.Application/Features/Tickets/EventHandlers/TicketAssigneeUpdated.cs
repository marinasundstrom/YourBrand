using YourBrand.Tenancy;
using YourBrand.Ticketing.Application.Common;

namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;

public sealed class TicketAssigneeUpdatedEventHandler(IApplicationDbContext context, ITicketRepository ticketRepository, IEmailService emailService, ITicketNotificationService ticketNotificationService, ISettableTenantContext tenantContext) : IDomainEventHandler<TicketAssigneeUpdated>
{
    public async Task Handle(TicketAssigneeUpdated notification, CancellationToken cancellationToken)
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