using Catalog.Services;

using MudBlazor;

using UserNotifications;

namespace Catalog.iOS;

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

        /*

        UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => {
            if (!approved)
                return;

            var content = new UNMutableNotificationContent()
            {
                Title = title,
                Body = body
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.25, false);
            var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => {
                if (err != null)
                    throw new System.Exception($"Failed to schedule notification: {err}");
            });
        });

        */
    }
}