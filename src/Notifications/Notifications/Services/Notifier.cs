
using MassTransit;

namespace YourBrand.Notifications.Services;

public class Notifier(IBus bus) : INotifier
{
    public async Task Notify()
    {
        //await _bus.Publish(new RandomNotification($"This message was sent at: {DateTime.Now}"));
    }
}