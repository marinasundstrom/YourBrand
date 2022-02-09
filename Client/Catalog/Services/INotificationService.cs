namespace Catalog.Services;

public interface INotificationService
{
    void ShowNotification(string title, string body);
}