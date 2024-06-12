using System.Security.Claims;

namespace ChatApp.Services;

public interface ICurrentUserService
{
    string? UserId { get; }

    string? ConnectionId { get; }

    bool IsInRole(string role);
}

public interface ICurrentUserServiceInternal : ICurrentUserService
{
    void SetUser(ClaimsPrincipal user);

    void SetConnectionId(string connectionId);
}
