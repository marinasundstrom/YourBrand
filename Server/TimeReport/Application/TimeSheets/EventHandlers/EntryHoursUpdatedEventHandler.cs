using System;

using MediatR;

using Microsoft.Extensions.Logging;

using YourCompany.TimeReport.Application.Common.Models;
using YourCompany.TimeReport.Domain.Events;

namespace YourCompany.TimeReport.Application.TimeSheets.EventHandlers;

public class EntryHoursUpdatedEventHandler : INotificationHandler<DomainEventNotification<EntryHoursUpdatedEvent>>
{
    private readonly ILogger<EntryHoursUpdatedEventHandler> _logger;

    public EntryHoursUpdatedEventHandler(ILogger<EntryHoursUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<EntryHoursUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hours for {notification.DomainEvent.EntryId} was updated to {notification.DomainEvent.Hours}.");

        return Task.CompletedTask;
    }
}