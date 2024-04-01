using Microsoft.AspNetCore.Http;

namespace YourBrand.Tenancy;

public sealed class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private TenantId? _tenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public TenantId? TenantId => _tenantId ??= _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

    public void SetTenantId(TenantId tenantId) => _tenantId = tenantId;
}