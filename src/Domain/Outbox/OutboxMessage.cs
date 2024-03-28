using System;

namespace YourBrand.Domain.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public required DateTime OccurredOnUtc { get; set; }

    public DateTime? ProcessedOnUtc { get; set; }

    public required string Type { get; set; } = null!;

    public required string Content { get; set; } = null!;

    public string? Error { get; set; }
}