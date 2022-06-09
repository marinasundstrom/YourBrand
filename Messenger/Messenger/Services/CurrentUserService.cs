using System.Security.Claims;

using YourBrand.Messenger.Application.Common.Interfaces;

namespace YourBrand.Messenger.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentUserId;
    private ClaimsPrincipal? _claimsPrincipal;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _currentUserId ??= (_claimsPrincipal ??= _httpContextAccessor.HttpContext?.User)?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Role => (_claimsPrincipal ??= _httpContextAccessor.HttpContext?.User)?.FindFirstValue(ClaimTypes.Role);

    public string? GetAccessToken() => (_claimsPrincipal ??= _httpContextAccessor.HttpContext?.User)?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

    public void SetCurrentUser(string userId)
    {
        if (_currentUserId is not null)
        {
            throw new Exception("User has already been set.");
        }
        _currentUserId = userId;
    }

    public void SetCurrentUser(ClaimsPrincipal claimsPrincipal)
    {
        if (_claimsPrincipal is not null)
        {
            throw new Exception("User has already been set.");
        }
        _claimsPrincipal = claimsPrincipal;
    }
}