using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Domain.Events;

public record OrganizationUpdated: DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationUpdated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
