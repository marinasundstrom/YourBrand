using System.Security.Claims;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private string? _currentUserId;
    private string? _organizationId;

    public string? UserId => _currentUserId ??= _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    public string? OrganizationId => _organizationId ??= _httpContextAccessor.HttpContext?.User?.FindFirst("organization")?.Value;
}
