using System;

using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Domain;

public static class UserDtoExtensions
{
    public static string? GetDisplayName(this User user) => !String.IsNullOrEmpty(user.DisplayName) ? user?.DisplayName : $"{user.FirstName} {user?.LastName}";
}