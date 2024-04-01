using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this User user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}