using System.Security.Claims;

using AspNetCore.Authentication.ApiKey;

namespace YourBrand.ApiKeys;

class ApiKey(string key, string owner, List<Claim>? claims = null) : IApiKey
{
    public string Key { get; } = key;
    public string OwnerName { get; } = owner;
    public IReadOnlyCollection<Claim> Claims { get; } = claims ?? new List<Claim>();
}