using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public class OrganizationCreated : DomainEvent
{
    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }

    public string OrganizationId { get; }
}
