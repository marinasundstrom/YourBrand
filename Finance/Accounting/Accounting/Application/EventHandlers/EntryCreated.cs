
using YourBrand.Accounting.Application.Common.Models;
using YourBrand.Accounting.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace YourBrand.Accounting.Application.EventHandlers;

public class EntryCreatedEventHandler : INotificationHandler<DomainEventNotification<EntryCreatedEvent>>
{
    private readonly ILogger<EntryCreatedEventHandler> _logger;

    public EntryCreatedEventHandler(ILogger<EntryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<EntryCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entry created: {notification.DomainEvent.EntryId}");

        return Task.CompletedTask;
    }
}