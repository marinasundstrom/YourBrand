using System;

using YourBrand.TimeReport.Client;

namespace YourBrand.TimeReport;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this YourBrand.AppService.Client.UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}