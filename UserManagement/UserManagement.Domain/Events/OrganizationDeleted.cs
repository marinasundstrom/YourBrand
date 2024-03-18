using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Domain.Events;

public record OrganizationDeleted: DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationDeleted(string organizationId)
    {
        OrganizationId = organizationId;
    }
}