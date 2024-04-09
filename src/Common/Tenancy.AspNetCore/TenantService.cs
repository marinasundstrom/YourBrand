using Microsoft.AspNetCore.Http;

namespace YourBrand.Tenancy;

public sealed class TenantContext(IHttpContextAccessor httpContextAccessor) : ISettableTenantContext
{
    private TenantId? _tenantId;

    public TenantId? TenantId => _tenantId ??= httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

    public void SetTenantId(TenantId tenantId) => _tenantId = tenantId;
}