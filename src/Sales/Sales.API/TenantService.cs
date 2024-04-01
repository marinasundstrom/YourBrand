namespace YourBrand.Sales;

public sealed class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _tenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TenantId => _tenantId ??= _httpContextAccessor.HttpContext?.User?.FindFirst("tenant_id")?.Value;

    public void SetTenantId(string tenantId) => _tenantId = tenantId;
}