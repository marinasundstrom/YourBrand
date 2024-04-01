using Contracts;

using MassTransit;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Consumers;

public class NotificationConsumer : IConsumer<NotificationDto>
{
    private readonly INotificationClient _notificationClient;

    public NotificationConsumer(INotificationClient notificationClient)
    {
        _notificationClient = notificationClient;
    }

    public async Task Consume(ConsumeContext<NotificationDto> context)
    {
        var notification = context.Message;

        var dto = new YourBrand.Notifications.Client.NotificationDto()
        {
            Id = notification.Id,
            Published = notification.Published.GetValueOrDefault(),
            Title = notification.Title,
            Text = notification.Text,
            Link = notification.Link,
            UserId = notification.UserId,
            IsRead = notification.IsRead,
            Created = notification.Created,
            CreatedBy = notification.CreatedBy,
            LastModified = notification.LastModified,
            LastModifiedBy = notification.LastModifiedBy
        };

        await _notificationClient.NotificationReceived(dto);
    }
}