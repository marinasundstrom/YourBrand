using Microsoft.Extensions.Logging;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Application.TimeSheets.EventHandlers;

public class EntryHoursUpdatedEventHandler : IDomainEventHandler<EntryHoursUpdatedEvent>
{
    private readonly ILogger<EntryHoursUpdatedEventHandler> _logger;

    public EntryHoursUpdatedEventHandler(ILogger<EntryHoursUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EntryHoursUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Hours for {notification.EntryId} was updated to {notification.Hours}.");

        return Task.CompletedTask;
    }
}