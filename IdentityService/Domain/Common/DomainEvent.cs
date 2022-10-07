using MediatR;

namespace YourBrand.IdentityService.Domain.Common;

public abstract class DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();
}