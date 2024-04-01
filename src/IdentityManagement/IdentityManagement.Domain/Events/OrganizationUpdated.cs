using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record OrganizationUpdated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationUpdated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}