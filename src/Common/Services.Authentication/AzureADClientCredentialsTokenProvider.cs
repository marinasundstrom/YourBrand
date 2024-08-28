using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace YourBrand.Services.Authentication;

/// <summary>
/// Provides token from Client Credentials in Azure AD.
/// </summary>
public class AzureADClientCredentialsTokenProvider(IConfiguration configuration, ILogger<AzureADClientCredentialsTokenProvider> logger) : ITokenProvider
{
    private static readonly Dictionary<string, string> accessTokens = new Dictionary<string, string>();

    public async Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default)
    {
        baseUrl = baseUrl.TrimEnd('/');

        ApiDef? api = GetApiDef(baseUrl);

        if (api is null)
        {
            logger.LogInformation("No API definition");

            return null;
        }

        if (!cached || !accessTokens.TryGetValue(baseUrl, out var accessToken))
        {
            accessToken = await RequestTokenAsync(api.Scope);

            logger.LogInformation("Retrieved new access token");

            accessTokens[baseUrl] = accessToken!;

            return accessToken!;
        }

        logger.LogInformation("Retrieved cached access token");

        return accessToken;
    }

    private ApiDef? GetApiDef(string baseUrl)
    {
        var apis = configuration.GetSection("AzureAd:StoreFront:Apis").Get<ApiDef[]>();
        return apis.FirstOrDefault(x => x.BaseUrl.TrimEnd('/').Contains(baseUrl));
    }

    async Task<string?> RequestTokenAsync(string scope)
    {
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
               .Create(configuration.GetValue<string>("AzureAd:ClientId"))
               .WithTenantId(configuration.GetValue<string>("AzureAd:TenantId"))
               .WithClientSecret(configuration.GetValue<string>("AzureAd:StoreFront:ClientCredentials:ClientSecret"))
               .Build();

        var c = app.AcquireTokenForClient([scope]);
        var x = await c.ExecuteAsync();
        return x.AccessToken;
    }

    sealed record ApiDef(string Name, string BaseUrl, string Scope);
}