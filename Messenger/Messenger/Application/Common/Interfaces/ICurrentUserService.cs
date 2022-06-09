using System.Security.Claims;

namespace YourBrand.Messenger.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }

    string? GetAccessToken();

    void SetCurrentUser(string userId);
    void SetCurrentUser(ClaimsPrincipal claimsPrincipal);
}

public static class CurrentUserServiceExtensions
{
    public static bool IsCurrentUser(this ICurrentUserService currentUserService, string userId)
    {
        return currentUserService.UserId == userId;
    }

    public static bool IsUserInRole(this ICurrentUserService currentUserService, string role)
    {
        return currentUserService.Role == role;
    }
}