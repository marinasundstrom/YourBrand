using System;

using YourBrand.IdentityService.Client;

namespace YourBrand.Portal;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this YourBrand.AppService.Client.UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}