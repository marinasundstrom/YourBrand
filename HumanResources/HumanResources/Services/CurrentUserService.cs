using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Services;

public class CurrentPersonService : ICurrentPersonService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentPersonService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? PersonId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}