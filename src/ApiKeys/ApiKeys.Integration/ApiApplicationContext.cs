using System.Security.Claims;

using IdentityModel;

using Microsoft.AspNetCore.Http;

namespace YourBrand.ApiKeys;

public class ApiApplicationContext : IApiApplicationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? applicationId;

    public ApiApplicationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? AppId => applicationId ??= _httpContextAccessor.HttpContext?.User?.FindFirstValue("AppId");

    public string? AppName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}