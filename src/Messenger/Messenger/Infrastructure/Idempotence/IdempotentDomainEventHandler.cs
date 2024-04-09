using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Common;
using YourBrand.Messenger.Infrastructure.Persistence;
using YourBrand.Messenger.Infrastructure.Persistence.Outbox;

namespace YourBrand.Messenger.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    MessengerContext dbContext) : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
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