using MediatR;

namespace YourBrand.ApiKeys.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}