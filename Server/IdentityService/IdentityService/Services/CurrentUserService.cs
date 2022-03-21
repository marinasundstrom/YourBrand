using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using YourCompany.IdentityService.Application.Common.Interfaces;

namespace YourCompany.IdentityService.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}