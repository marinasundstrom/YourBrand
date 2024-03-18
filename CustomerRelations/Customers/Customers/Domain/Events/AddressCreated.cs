using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record AddressCreated(string AddressId) : DomainEvent;