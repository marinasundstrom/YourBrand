using System;

using Contracts;

using MassTransit;

namespace Worker.Services;

public class Notifier : INotifier
{
    private readonly IBus _bus;

    public Notifier(IBus bus)
    {
        _bus = bus;
    }

    public async Task Notify()
    {
        await _bus.Publish(new RandomNotification($"This message was sent at: {DateTime.Now}"));
    }
}