using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Infrastructure.Persistence;
using YourBrand.TimeReport.Infrastructure.Persistence.Outbox;

namespace YourBrand.TimeReport.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decorated,
    TimeReportContext dbContext) : IDomainEventHandler<TDomainEvent>
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