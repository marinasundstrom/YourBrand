namespace YourBrand.Accounting.Infrastructure.Persistence.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }

    public DateTimeOffset OccurredOnUtc { get; set; }

    public DateTimeOffset? ProcessedOnUtc { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Error { get; set; }
}