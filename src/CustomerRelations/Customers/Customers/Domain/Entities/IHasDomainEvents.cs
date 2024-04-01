namespace YourBrand.Customers.Domain.Entities;
using YourBrand.Customers.Domain.Common;

public interface IHasDomainEvents
{
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }

    void AddDomainEvent(DomainEvent domainEvent);

    void RemoveDomainEvent(DomainEvent domainEvent);

    void ClearDomainEvents();
}