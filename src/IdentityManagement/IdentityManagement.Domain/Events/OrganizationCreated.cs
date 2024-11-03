using YourBrand.Domain;
using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record OrganizationCreated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}