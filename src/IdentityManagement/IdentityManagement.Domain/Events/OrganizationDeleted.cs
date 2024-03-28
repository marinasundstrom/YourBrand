using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record OrganizationDeleted: DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationDeleted(string organizationId)
    {
        OrganizationId = organizationId;
    }
}