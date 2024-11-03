using YourBrand.Customers.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Customers.Domain.Events;

public record AddressCreated(string AddressId) : DomainEvent;