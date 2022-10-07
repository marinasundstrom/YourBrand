using MediatR;

namespace YourBrand.RotRutService.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}