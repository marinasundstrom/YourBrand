using MediatR;

namespace YourBrand.Messenger.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
}