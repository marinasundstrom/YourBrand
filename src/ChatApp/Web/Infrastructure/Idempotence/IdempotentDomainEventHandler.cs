﻿using Microsoft.EntityFrameworkCore;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Infrastructure.Persistence;
using ChatApp.Infrastructure.Persistence.Outbox;

namespace ChatApp.Infrastructure.Idempotence;

public sealed class IdempotentDomainEventHandler<TDomainEvent> : IDomainEventHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{
    private readonly IDomainEventHandler<TDomainEvent> decorated;
    private readonly ApplicationDbContext dbContext;

    public IdempotentDomainEventHandler(
        IDomainEventHandler<TDomainEvent> decorated,
        ApplicationDbContext dbContext)
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

