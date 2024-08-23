namespace YourBrand.Identity;

public static class UserContextExtensions
{
    public static bool IsCurrentUser(this IUserContext userContext, string userId)
    {
        return userContext.UserId == userId;
    }
}