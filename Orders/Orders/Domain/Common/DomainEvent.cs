using MediatR;

namespace YourBrand.Orders.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}