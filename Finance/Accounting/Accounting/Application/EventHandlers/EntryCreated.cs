
using Accounting.Application.Common.Models;
using Accounting.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Accounting.Application.EventHandlers;

public class EntryCreatedEventHandler : INotificationHandler<DomainEventNotification<EntryCreatedEvent>>
{
    private readonly ILogger<EntryCreatedEventHandler> _logger;

    public EntryCreatedEventHandler(ILogger<EntryCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<EntryCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Entry created: {notification.DomainEvent.Entry.Id}");

        return Task.CompletedTask;
    }
}