using MediatR;

namespace YourBrand.Invoicing.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}