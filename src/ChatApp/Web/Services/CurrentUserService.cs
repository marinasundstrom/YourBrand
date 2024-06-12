using System.Security.Claims;

namespace ChatApp.Web.Services;

public sealed class CurrentUserService : ICurrentUserService, ICurrentUserServiceInternal
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? _user;
    private string? _currentUserId;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _currentUserId ??= GetUser()?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public bool IsInRole(string role) => GetUser().IsInRole(role);

    public string? ConnectionId { get; private set; }

    private ClaimsPrincipal GetUser()
    {
        return _user ??= _httpContextAccessor.HttpContext?.User!;
    }

    public void SetUser(ClaimsPrincipal user)
    {
        _user = user;
    }

    public void SetConnectionId(string connectionId)
    {
        ConnectionId = connectionId;
    }
}