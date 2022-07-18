using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class OrganizationCreated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
