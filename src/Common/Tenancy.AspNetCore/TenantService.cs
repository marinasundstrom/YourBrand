using Microsoft.AspNetCore.Http;

namespace YourBrand.Tenancy;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? OrganizationId => _httpContextAccessor?.HttpContext?.User?.FindFirst("organizationId")?.Value;
}