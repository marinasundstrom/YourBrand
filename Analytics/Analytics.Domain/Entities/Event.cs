using System;
using YourBrand.Analytics.Domain.Events;
using YourBrand.Analytics.Domain.Enums;

namespace YourBrand.Analytics.Domain.Entities;

public class Event : Entity<string>
{
#nullable disable

    protected Event() : base() { }

#nullable restore

    public Event(string clientId, string sessionId, EventType eventType, string data)
    : base(Guid.NewGuid().ToString())
    {
        ClientId = clientId;
        SessionId = sessionId;
        EventType = eventType;
        Data = data;
    }

    public string ClientId { get; private set; } = default!;

    public string SessionId { get; private set; } = default!;

    public EventType EventType { get; private set; }

    public DateTimeOffset DateTime { get; private set; } = DateTimeOffset.UtcNow;

    public string Data { get; private set; }
}
