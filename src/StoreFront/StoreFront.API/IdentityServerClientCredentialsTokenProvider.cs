using IdentityModel.Client;

/// <summary>
/// Provides token from Client Credentials in Identity Server.
/// </summary>
public class IdentityServerClientCredentialsTokenProvider : ITokenProvider
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityServerClientCredentialsTokenProvider> _logger;

    public IdentityServerClientCredentialsTokenProvider(IConfiguration configuration, ILogger<IdentityServerClientCredentialsTokenProvider> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    private string? cachedAccessToken;

    public async Task<string?> RequestTokenAsync(string baseUrl, bool cached = true, CancellationToken cancellationToken = default)
    {
        if (!cached || cachedAccessToken is null)
        {
            var accessToken = await RequestTokenAsync();

            _logger.LogInformation("Retrieved new access token");

            cachedAccessToken = accessToken!;

            return accessToken!;

        }

        _logger.LogInformation("Retrieved cached access token");

        return cachedAccessToken;
    }

    async Task<string?> RequestTokenAsync()
    {
        // discover endpoints from metadata
        var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync(_configuration.GetValue<string>("StoreFront:Authority"));
        if (disco.IsError)
        {
            Console.WriteLine(disco.Error);
            throw new Exception();
        }

        // request token
        TokenResponse tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = _configuration.GetValue<string>("StoreFront:ClientCredentials:ClientId")!,
            ClientSecret = _configuration.GetValue<string>("StoreFront:ClientCredentials:ClientSecret"),
            Scope = _configuration.GetValue<string>("StoreFront:Scope"),
        });
        if (tokenResponse.IsError)
        {
            Console.WriteLine(tokenResponse.Error);
        }

        Console.WriteLine(tokenResponse.Json);

        return tokenResponse.AccessToken;
    }
}