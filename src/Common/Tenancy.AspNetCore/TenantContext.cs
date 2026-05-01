using Microsoft.AspNetCore.Http;

namespace YourBrand.Tenancy;

public sealed class TenantContext(IHttpContextAccessor httpContextAccessor) : ISettableTenantContext
{
    private const string TenantIdHeader = "TenantId";
    private const string TenantIdClaim = "tenant_id";
    private const string ClientTenantIdClaim = "client_tenant_id";

    private TenantId? _tenantId;

    public TenantId? TenantId => _tenantId ??= GetTenantIdFromRequest();

    public void SetTenantId(TenantId tenantId)
    {
        _tenantId = string.IsNullOrWhiteSpace(tenantId.Value)
            ? default(TenantId?)
            : tenantId;
    }

    private TenantId? GetTenantIdFromRequest()
    {
        var httpContext = httpContextAccessor.HttpContext;

        return GetTenantId(httpContext?.Request.Headers[TenantIdHeader].FirstOrDefault())
            ?? GetTenantId(httpContext?.User.FindFirst(TenantIdClaim)?.Value)
            ?? GetTenantId(httpContext?.User.FindFirst(ClientTenantIdClaim)?.Value);
    }

    private static TenantId? GetTenantId(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
