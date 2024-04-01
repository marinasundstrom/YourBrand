using System.Security.Claims;

namespace YourBrand.Sales;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private string? _currentUserId;

    public string? UserId => _currentUserId ??= _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}