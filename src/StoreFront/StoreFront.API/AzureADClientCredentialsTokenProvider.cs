using Microsoft.Identity.Client;

/// <summary>
/// Provides token from Client Credentials in Azure AD.
/// </summary>
public class AzureADClientCredentialsTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AzureADClientCredentialsTokenProvider> _logger;

    public AzureADClientCredentialsTokenProvider(IConfiguration configuration, ILogger<AzureADClientCredentialsTokenProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private static readonly Dictionary<string, string> accessTokens = new Dictionary<string, string>();

    public async Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default)
    {
        baseUrl = baseUrl.TrimEnd('/');

        ApiDef? api = GetApiDef(baseUrl);

        if (api is null)
        {
            _logger.LogInformation("No API definition");

            return null;
        }

        if (!cached || !accessTokens.TryGetValue(baseUrl, out var accessToken))
        {
            accessToken = await RequestTokenAsync(baseUrl, api.Scope);

            _logger.LogInformation("Retrieved new access token");

            accessTokens[baseUrl] = accessToken!;

            return accessToken!;
        }

        _logger.LogInformation("Retrieved cached access token");

        return accessToken;
    }

    private ApiDef? GetApiDef(string baseUrl)
    {
        var apis = _configuration.GetSection("AzureAd:StoreFront:Apis").Get<ApiDef[]>();
        return apis.FirstOrDefault(x => x.BaseUrl.TrimEnd('/').Contains(baseUrl));
    }

    async Task<string?> RequestTokenAsync(string baseUrl, string scope)
    {
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
               .Create(_configuration.GetValue<string>("AzureAd:ClientId"))
               .WithTenantId(_configuration.GetValue<string>("AzureAd:TenantId"))
               .WithClientSecret(_configuration.GetValue<string>("AzureAd:StoreFront:ClientCredentials:ClientSecret"))
               .Build();

        var c = app.AcquireTokenForClient([scope]);
        var x = await c.ExecuteAsync();
        return x.AccessToken;
    }

    record ApiDef(string Name, string BaseUrl, string Scope);
}