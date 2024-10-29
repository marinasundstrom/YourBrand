using YourBrand.HumanResources.Client;

namespace YourBrand.HumanResources;

public static class PersonExtensions
{
    //public static string? GetDisplayName(this YourBrand.AppService.Client.User person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    public static string? GetDisplayName(this Person person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";

    public static string? GetDisplayName(this Person2 person) => !String.IsNullOrEmpty(person.DisplayName) ? person?.DisplayName : $"{person.FirstName} {person?.LastName}";
}