
using Hangfire;

using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Domain.Events;

namespace YourBrand.Notifications.Application.Notifications.EventHandlers;

public class NotificationCreatedEventHandler : IDomainEventHandler<NotificationCreatedEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Common.Interfaces.INotificationPublisher _notficationPublisher;
    private readonly IBackgroundJobClient _recurringJobManager;

    public NotificationCreatedEventHandler(IServiceProvider serviceProvider, Common.Interfaces.INotificationPublisher notficationSender, IBackgroundJobClient recurringJobManager)
    {
        _serviceProvider = serviceProvider;
        _notficationPublisher = notficationSender;
        _recurringJobManager = recurringJobManager;
    }

    public async Task Handle(NotificationCreatedEvent notification2, CancellationToken cancellationToken)
    {
        var domainEvent = notification2;

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IWorkerContext>();

        var notification = await context.Notifications
            .FirstAsync(i => i.Id == domainEvent.NotificationId, cancellationToken);

        if (notification.ScheduledFor is not null)
        {
            if (notification.ScheduledFor < DateTimeOffset.UtcNow)
            {
                throw new InvalidOperationException("Cannot send notification back in time!");
            }

            await ScheduleNotification(context, notification);
        }
        else
        {
            await PublishNotification(notification);
        }
    }

    private async Task PublishNotification(Domain.Entities.Notification notification)
    {
        await _notficationPublisher.PublishNotification(notification);
    }

    private async Task ScheduleNotification(IWorkerContext context, Domain.Entities.Notification notification)
    {
        var delay = notification.ScheduledFor.GetValueOrDefault() - DateTime.Now;

        var jobId = _recurringJobManager.Schedule<Common.Interfaces.INotificationPublisher>(
            (sender) => sender.PublishNotification(notification),
                delay);

        notification.ScheduledJobId = jobId;

        await context.SaveChangesAsync();
    }
}