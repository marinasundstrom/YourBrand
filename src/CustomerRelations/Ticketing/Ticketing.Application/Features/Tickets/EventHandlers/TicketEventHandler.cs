namespace YourBrand.Ticketing.Application.Features.Tickets.EventHandlers;


/*

public sealed class TicketEventHandler(IApplicationDbContext context) : IDomainEventHandler<TicketDomainEvent>
{
    public async Task Handle(TicketDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("STARTING");

        var ev = new TicketEvent();
        ev.OrganizationId = notification.OrganizationId;
        ev.TicketId = notification.TicketId;
        ev.Event = System.Text.Json.JsonSerializer.Serialize<TicketDomainEvent>(notification);

        context.TicketEvents.Add(ev);

        await context.SaveChangesAsync(cancellationToken);

        Console.WriteLine("SAVED");
    }
}

*/