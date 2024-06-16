using YourBrand.Notifications.Client;

namespace YourBrand.Notifications;

public interface INotificationService
{
    Task PublishNotificationAsync(Notification notification);
}

public sealed class NotificationService(INotificationsClient notificationClients) : INotificationService
{
    public async Task PublishNotificationAsync(Notification notification)
    {
        await notificationClients.CreateNotificationAsync(new CreateNotification
        {
            Content = notification.Content,
            Link = notification.Link,
            UserId = notification.UserId,
            ScheduledFor = notification.ScheduledFor
        });
    }
}

public record class Notification(string Content)
{
    public string? Link { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset? ScheduledFor { get; set; }
}