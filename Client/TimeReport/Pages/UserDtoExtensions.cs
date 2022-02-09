using System;

using TimeReport.Client;

namespace TimeReport.Pages;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

}