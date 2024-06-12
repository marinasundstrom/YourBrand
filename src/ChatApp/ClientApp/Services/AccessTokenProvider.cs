using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace ChatApp.Services;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
}

public sealed class AccessTokenProvider : IAccessTokenProvider
{
    private readonly Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider _accessTokenProvider;

    public AccessTokenProvider(Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var results = await _accessTokenProvider.RequestAccessToken(new AccessTokenRequestOptions() { Scopes = new[] { "myapi" } });

        if (results.TryGetToken(out var accessToken))
        {
            return accessToken.Value;
        }

        return null;
    }
}
