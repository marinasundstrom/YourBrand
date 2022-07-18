using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class OrganizationUpdated: DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationUpdated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
