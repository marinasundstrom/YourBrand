using MudBlazor;


namespace YourBrand.Portal.Services;

public class NotificationService(ISnackbar snackbar) : INotificationService
{
    public void ShowNotification(string title, string body)
    {
        snackbar.Add(body);
    }
}