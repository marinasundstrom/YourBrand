using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace YourBrand.Portal.Services;

public class AccessTokenProvider(Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider accessTokenProvider) : YourBrand.Portal.Services.IAccessTokenProvider
{
    public async Task<string?> GetAccessTokenAsync()
    {
        var results = await accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions() { Scopes = new[] { "myapi" } });

        if (results.TryGetToken(out var accessToken))
        {
            return accessToken.Value;
        }

        return null;
    }
}