using YourBrand.IdentityService.Domain.Common;

namespace YourBrand.IdentityService.Domain.Common.Interfaces;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    void AddDomainEvent(DomainEvent domainEvent);
    void ClearDomainEvents();
    void RemoveDomainEvent(DomainEvent domainEvent);
}