using MediatR;

namespace YourBrand.Payments.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}