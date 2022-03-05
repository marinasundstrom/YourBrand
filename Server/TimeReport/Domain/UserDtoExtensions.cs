using System;

using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Domain;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this User user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}