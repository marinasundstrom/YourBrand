using Skynet.Services;

using MudBlazor;


namespace Skynet.Services;

public class NotificationService : INotificationService
{
    private readonly ISnackbar snackbar;

    public NotificationService(ISnackbar snackbar)
    {
        this.snackbar = snackbar;
    }

    public void ShowNotification(string title, string body)
    {
        snackbar.Add(body);
    }
}