using MediatR;

namespace YourBrand.Warehouse.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}