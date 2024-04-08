using System.Security.Claims;

namespace YourBrand.Identity;

public interface ISettableUserContext : IUserContext
{
    void SetCurrentUser(UserId userId);
    void SetCurrentUser(ClaimsPrincipal claimsPrincipal);
}