using YourBrand.Customers.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Customers.Domain.Events;

public record PersonCreated(string PersonId) : DomainEvent;