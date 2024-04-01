using Microsoft.EntityFrameworkCore;

using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Domain.Common;
using YourBrand.RotRutService.Infrastructure.Persistence;
using YourBrand.RotRutService.Infrastructure.Persistence.Outbox;

namespace YourBrand.RotRutService.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    private readonly IDomainEventHandler<TDomainEvent> decorated;
    private readonly RotRutContext dbContext;

    public IdempotentDomainEventHandler(
        IDomainEventHandler<TDomainEvent> decorated,
        RotRutContext dbContext)
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