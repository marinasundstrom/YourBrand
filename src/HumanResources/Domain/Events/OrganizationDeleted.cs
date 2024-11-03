using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record OrganizationDeleted : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationDeleted(string organizationId)
    {
        OrganizationId = organizationId;
    }
}