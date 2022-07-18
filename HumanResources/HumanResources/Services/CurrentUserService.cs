using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Services;

public class CurrentPersonService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentPersonService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}