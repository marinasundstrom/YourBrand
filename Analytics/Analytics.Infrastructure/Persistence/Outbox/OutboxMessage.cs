using System;

namespace YourBrand.Analytics.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Error { get; set; }
}
