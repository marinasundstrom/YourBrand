using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record OrganizationCreated : DomainEvent
{
    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }

    public string OrganizationId { get; }
}
