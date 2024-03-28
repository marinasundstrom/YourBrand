using YourBrand.Sales.Features.Common;

public sealed class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _tenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? TenantId => _tenantId ??= _httpContextAccessor.HttpContext?.User?.FindFirst("organization")?.Value;

    //     public string? TenantId => _httpContextAccessor?.HttpContext?.Request.Headers["TenantId"].SingleOrDefault();
}