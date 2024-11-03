using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record OrganizationUpdated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationUpdated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}