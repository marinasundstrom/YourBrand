using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Contracts;
using YourBrand.Notifications.Domain.Entities;

namespace YourBrand.Notifications.Services;

public class NotificationPublisher(IBus bus, IServiceProvider serviceProvider, ILogger<NotificationPublisher> logger) : INotificationPublisher
{
    public async Task PublishNotification(Notification n1)
    {
        logger.LogInformation("Sending notification");

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IWorkerContext>();

        var notification = await context.Notifications.FirstAsync(n => n.Id == n1.Id);
        notification.Published = DateTime.Now;
        await context.SaveChangesAsync();

        var notifcationDto = new NotificationDto(notification.Id, notification.Published, notification.Content, notification.Link, notification.UserId, notification.IsRead, notification.Created, notification.CreatedById, notification.LastModified, notification.LastModifiedById);

        await bus.Publish(notifcationDto);
    }
}