using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record TenantCreated : DomainEvent
{
    public string TenantId { get; set; }

    public TenantCreated(string tenantId)
    {
        TenantId = tenantId;
    }
}