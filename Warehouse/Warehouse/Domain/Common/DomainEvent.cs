using MediatR;

namespace YourBrand.Warehouse.Domain.Common;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}