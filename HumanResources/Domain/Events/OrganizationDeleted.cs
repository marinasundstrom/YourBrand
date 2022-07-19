using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public class OrganizationDeleted: DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationDeleted(string organizationId)
    {
        OrganizationId = organizationId;
    }
}