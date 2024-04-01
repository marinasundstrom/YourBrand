using System.Security.Claims;

namespace YourBrand.Identity;

public interface ICurrentUserService
{
    UserId? UserId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    string? Role { get; }

    string? GetAccessToken();

    void SetCurrentUser(UserId userId);
    void SetCurrentUser(ClaimsPrincipal claimsPrincipal);
}