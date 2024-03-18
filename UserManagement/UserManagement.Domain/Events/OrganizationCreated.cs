using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Domain.Events;

public record OrganizationCreated : DomainEvent
{
    public string OrganizationId { get; set; }

    public OrganizationCreated(string organizationId)
    {
        OrganizationId = organizationId;
    }
}
