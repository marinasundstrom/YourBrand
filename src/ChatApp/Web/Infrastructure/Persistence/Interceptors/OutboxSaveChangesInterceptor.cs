using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using ChatApp.Infrastructure.Persistence.Outbox;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.Interceptors;

public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService currentUserService;

    public OutboxSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        this.currentUserService = currentUserService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;

        if (context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entities = context.ChangeTracker
                        .Entries<IHasDomainEvents>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .OrderBy(e => e.Timestamp)
            .ToList();

        var currentUserId = currentUserService.UserId;

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            domainEvent.CurrentUserId = currentUserId is not null ? (UserId)currentUserId! : null!;

            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        context.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}