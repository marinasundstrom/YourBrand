using System;

using YourBrand.HumanResources.Client;

namespace YourBrand.HumanResources;

public static class PersonDtoExtensions
{
    //public static string? GetDisplayName(this YourBrand.AppService.Client.User person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    public static string? GetDisplayName(this PersonDto person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    public static string? GetDisplayName(this Person2Dto person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";
}