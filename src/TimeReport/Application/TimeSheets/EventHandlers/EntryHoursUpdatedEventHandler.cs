using Microsoft.Extensions.Logging;

using YourBrand.Domain;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Application.TimeSheets.EventHandlers;

public class EntryHoursUpdatedEventHandler(ILogger<EntryHoursUpdatedEventHandler> logger) : IDomainEventHandler<EntryHoursUpdatedEvent>
{
    public Task Handle(EntryHoursUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Hours for {notification.EntryId} was updated to {notification.Hours}.");

        return Task.CompletedTask;
    }
}