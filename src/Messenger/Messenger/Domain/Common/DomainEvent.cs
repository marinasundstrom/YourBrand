using MediatR;

namespace YourBrand.Messenger.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Timestamp { get; } = DateTime.UtcNow;
}