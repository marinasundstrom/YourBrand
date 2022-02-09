using System;

using Contracts;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;
using Worker.Application.Common.Interfaces;
using Worker.Domain.Entities;

namespace Worker.Services;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationPublisher> _logger;

    public NotificationPublisher(IBus bus, IServiceProvider serviceProvider, ILogger<NotificationPublisher> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishNotification(Notification n1)
    {
        _logger.LogInformation("Sending notification");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IWorkerContext>();

        var notification = await context.Notifications.FirstAsync(n => n.Id == n1.Id);
        notification.Published = DateTime.Now;
        await context.SaveChangesAsync();

        var notifcationDto = new NotificationDto(notification.Id, notification.Published, notification.Title, notification.Text, notification.Link, notification.UserId, notification.IsRead, notification.Created, notification.CreatedBy, notification.LastModified, notification.LastModifiedBy);

        await _bus.Publish(notifcationDto);
    }
}