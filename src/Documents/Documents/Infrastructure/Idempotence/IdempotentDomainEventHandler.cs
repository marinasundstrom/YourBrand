using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Infrastructure.Persistence;
using YourBrand.Documents.Infrastructure.Persistence.Outbox;

namespace YourBrand.Documents.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    DocumentsContext dbContext) : IDomainEventHandler<TDomainEvent>
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