namespace YourBrand.Identity;

public static class UserContextExtensions
{
    public static bool IsCurrentUser(this IUserContext userContext, string userId)
    {
        return userContext.UserId == userId;
    }

    public static bool IsUserInRole(this IUserContext userContext, string role)
    {
        return userContext.Role == role;
    }
}