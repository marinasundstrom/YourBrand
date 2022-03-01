using System;

using Skynet.IdentityService.Client;

namespace Skynet.Portal;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this Skynet.Client.UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}