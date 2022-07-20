using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace YourBrand.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _currentUserId;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _currentUserId ??= _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? GetAccessToken() => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;

    public void SetCurrentUser(string userId)
    {
        if (_currentUserId is not null)
        {
            throw new Exception("User has already been set.");
        }
        _currentUserId = userId;
    }

    public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
}