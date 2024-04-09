using IdentityModel.Client;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace YourBrand.Authentication;

/// <summary>
/// Provides token from Client Credentials in Identity Server.
/// </summary>
public class IdentityServerClientCredentialsTokenProvider(IConfiguration configuration, ILogger<IdentityServerClientCredentialsTokenProvider> logger) : ITokenProvider
{
    private string? cachedAccessToken;

    public async Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default)
    {
        if (!cached || cachedAccessToken is null)
        {
            var accessToken = await RequestTokenAsync();

            logger.LogInformation("Retrieved new access token");

            cachedAccessToken = accessToken!;

            return accessToken!;

        }

        logger.LogInformation("Retrieved cached access token");

        return cachedAccessToken;
    }

    async Task<string?> RequestTokenAsync()
    {
        // discover endpoints from metadata
        var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync(configuration.GetValue<string>("Local:Authority"));
        if (disco.IsError)
        {
            Console.WriteLine(disco.Error);
            throw new Exception();
        }

        // request token
        TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = configuration.GetValue<string>("Local:ClientCredentials:ClientId")!,
            ClientSecret = configuration.GetValue<string>("Local:ClientCredentials:ClientSecret"),
            Scope = configuration.GetValue<string>("Local:Scope"),
        });
        if (tokenResponse.IsError)
        {
            Console.WriteLine(tokenResponse.Error);
        }

        Console.WriteLine(tokenResponse.Json);

        return tokenResponse.AccessToken;
    }
}