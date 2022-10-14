using MediatR;

namespace YourBrand.Inventory.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}