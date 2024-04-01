using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record TenantUpdated: DomainEvent
{
    public string TenantId { get; set; }

    public TenantUpdated(string tenantId)
    {
        TenantId = tenantId;
    }
}
