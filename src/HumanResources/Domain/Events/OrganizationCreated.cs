using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record OrganizationCreated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
