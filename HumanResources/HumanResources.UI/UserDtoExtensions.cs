using System;

using YourBrand.HumanResources.Client;

namespace YourBrand.HumanResources;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this YourBrand.AppService.Client.UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}