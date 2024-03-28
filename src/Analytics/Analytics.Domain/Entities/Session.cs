using System;
using YourBrand.Analytics.Domain.ValueObjects;

namespace YourBrand.Analytics.Domain.Entities;

public class Session : Entity<string>
{
#nullable disable

    protected Session() : base() { }

#nullable restore

    public Session(string clientId, string? ipAddress, DateTimeOffset started)
    : base(Guid.NewGuid().ToString())
    {
        ClientId = clientId;
        IPAddress = ipAddress;
        Started = started;
        Expires = started.AddMinutes(30);
    }

    public string ClientId { get; private set; } = default!;

    public Client Client { get; private set; } = default!;

    public string? IPAddress { get; private set; }

    public Coordinates? Coordinates { get; set; }

    public DateTimeOffset Started { get; private set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset Expires { get; set; }
}
