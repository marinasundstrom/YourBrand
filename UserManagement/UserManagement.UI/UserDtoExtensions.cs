using System;

using YourBrand.UserManagement.Client;

namespace YourBrand.UserManagement;

public static class UserExtensions
{
    //public static string? GetDisplayName(this YourBrand.AppService.Client.User person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    public static string? GetDisplayName(this User person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    //public static string? GetDisplayName(this User2 person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";
}