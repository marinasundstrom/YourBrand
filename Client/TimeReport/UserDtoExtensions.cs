using System;

using Skynet.TimeReport.Client;

namespace Skynet.TimeReport;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this UserDto user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";

}