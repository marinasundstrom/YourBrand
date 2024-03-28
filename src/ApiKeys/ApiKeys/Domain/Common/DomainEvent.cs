using MediatR;

namespace YourBrand.ApiKeys.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime Timestamp { get; } = DateTime.UtcNow;
}