using System.Net;
using System.Security.Claims;

namespace YourBrand.StoreFront.API;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private string? _currentUserId;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private string? host;

    public string? UserId => _currentUserId ??= _httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string? ClientId => _httpContext.Request.Headers["X-Client-Id"].FirstOrDefault();

    public string? SessionId => _httpContext?.Request.Headers["X-Session-Id"].FirstOrDefault();

    public int? CustomerNo
    {
        get
        {
            var str = _httpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "CustomerId")?.Value;

            if (str is null) return null;

            return int.Parse(str);
        }
    }

    public string? Host
    {
        get
        {
            var parts = _httpContext?.Request.Host.Host.Split('.');
            if (parts!.Count() > 2)
            {
                return host ??= parts!.First();
            }
            return null;
        }
    }

    public string? UserAgent => _httpContext?.Request.Headers.UserAgent.ToString();

    public string? GetRemoteIPAddress(bool allowForwarded = true)
    {
        if (allowForwarded)
        {
            string header = (_httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? _httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault())!;
            if (IPAddress.TryParse(header, out IPAddress? ip))
            {
                return ip.ToString();
            }
        }
        return _httpContext.Connection.RemoteIpAddress?.ToString();
    }
}