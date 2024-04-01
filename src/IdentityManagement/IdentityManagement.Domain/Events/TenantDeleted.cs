using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record TenantDeleted: DomainEvent
{
    public string TenantId { get; set; }

    public TenantDeleted(string tenantId)
    {
        TenantId = tenantId;
    }
}