using System;

using Skynet.IdentityService.Client;

namespace Skynet.Portal.Users;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

}