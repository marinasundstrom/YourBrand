using System.Security.Claims;

namespace YourBrand.Identity;

public interface IUserContext
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