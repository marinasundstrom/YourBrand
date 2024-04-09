using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace YourBrand.ApiKeys;

public class ApiApplicationContext(IHttpContextAccessor httpContextAccessor) : IApiApplicationContext
{
    private string? applicationId;

    public string? AppId => applicationId ??= httpContextAccessor.HttpContext?.User?.FindFirstValue("AppId");

    public string? AppName => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}