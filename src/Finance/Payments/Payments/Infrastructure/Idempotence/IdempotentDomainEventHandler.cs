using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Domain.Common;
using YourBrand.Payments.Infrastructure.Persistence;
using YourBrand.Payments.Infrastructure.Persistence.Outbox;

namespace YourBrand.Payments.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    private readonly IDomainEventHandler<TDomainEvent> decorated;
    private readonly PaymentsContext dbContext;

    public IdempotentDomainEventHandler(
        IDomainEventHandler<TDomainEvent> decorated,
        PaymentsContext dbContext)
    {
        this.decorated = decorated;
        this.dbContext = dbContext;
    }

    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        string consumer = decorated.GetType().Name;

        if (await dbContext.Set<OutboxMessageConsumer>()
            .AnyAsync(
                outboxMessageConsumer =>
                    outboxMessageConsumer.Id == notification.Id &&
                    outboxMessageConsumer.Consumer == consumer,
                cancellationToken))
        {
            return;
        }

        await decorated.Handle(notification, cancellationToken);

        dbContext.Set<OutboxMessageConsumer>()
            .Add(new OutboxMessageConsumer
            {
                Id = notification.Id,
                Consumer = consumer
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}