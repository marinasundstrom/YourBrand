
using Microsoft.Extensions.Logging;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Events;

namespace YourBrand.Accounting.Application.EventHandlers;

public class EntryCreatedEventHandler(ILogger<EntryCreatedEventHandler> logger) : IDomainEventHandler<EntryCreatedEvent>
{
    public Task Handle(EntryCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Entry created: {notification.EntryId}");

        return Task.CompletedTask;
    }
}