using System;

using YourBrand.TimeReport.Client;

namespace YourBrand.TimeReport;

public static class UserExtensions
{
    public static string? GetDisplayName(this User user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}