using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace YourBrand.Identity;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private UserId? _currentUserId;
    private ClaimsPrincipal? _claimsPrincipal;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _claimsPrincipal = _httpContextAccessor.HttpContext?.User;
    }

    public UserId? UserId => _currentUserId ??= _claimsPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetAccessToken() => _claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

    public void SetCurrentUser(UserId userId)
    {
        if (_currentUserId is not null)
        {
            throw new Exception("User has already been set.");
        }
        _currentUserId = userId;
    }

    public string? FirstName => _claimsPrincipal?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName => _claimsPrincipal?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? Email => _claimsPrincipal?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => _claimsPrincipal?.FindFirst(ClaimTypes.Role)?.Value;

    public void SetCurrentUser(ClaimsPrincipal claimsPrincipal)
    {
        if (_claimsPrincipal is not null)
        {
            throw new Exception("User has already been set.");
        }
        _claimsPrincipal = claimsPrincipal;
    }
}