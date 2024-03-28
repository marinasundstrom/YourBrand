namespace YourBrand.Identity;

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