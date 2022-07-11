using System;
using System.Security.Claims;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Infrastructure;

namespace YourBrand.Notifications.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
}