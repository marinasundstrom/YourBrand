using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record OrganizationCreated(string OrganizationId) : DomainEvent;