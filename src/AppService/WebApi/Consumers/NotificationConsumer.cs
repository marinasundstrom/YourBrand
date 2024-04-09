using MassTransit;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Consumers;

public class NotificationConsumer(INotificationClient notificationClient) : IConsumer<YourBrand.Notifications.Contracts.NotificationDto>
{
    public async Task Consume(ConsumeContext<YourBrand.Notifications.Contracts.NotificationDto> context)
    {
        var notification = context.Message;

        var dto = new YourBrand.Notifications.Client.Notification()
        {
            Id = notification.Id,
            Published = notification.Published.GetValueOrDefault(),
            Content = notification.Content,
            Link = notification.Link,
            UserId = notification.UserId,
            IsRead = notification.IsRead,
            Created = notification.Created,
            CreatedBy = notification.CreatedBy,
            LastModified = notification.LastModified,
            LastModifiedBy = notification.LastModifiedById
        };

        await notificationClient.NotificationReceived(dto);
    }
}