using YourBrand.ChatApp;

namespace YourBrand.ChatApp;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this User user) => user.Name; //!String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

    public static string GetInitials(this string name)
    {
        var nameParts = name.Split(" ");
        if (nameParts.Length == 0 || nameParts.Length == 1)
        {
            return name[0].ToString();
        }
        return $"{nameParts[0][0]}{nameParts[1][0]}";
    }
}