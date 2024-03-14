using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record PersonCreated(string PersonId) : DomainEvent;