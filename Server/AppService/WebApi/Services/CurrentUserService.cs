using System;
using System.Security.Claims;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Infrastructure;

namespace YourBrand.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentUserId;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _currentUserId ??= _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public string? GetAccessToken() => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

    public void SetCurrentUser(string userId)
    {
        if(_currentUserId is not null)
        {
            throw new Exception("User has already been set.");
        }
        _currentUserId = userId;
    }
}
