using System;

using YourCompany.TimeReport.Client;

namespace YourCompany.TimeReport;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this YourCompany.AppService.Client.UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}