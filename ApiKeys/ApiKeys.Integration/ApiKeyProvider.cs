
using System.Security.Claims;

using AspNetCore.Authentication.ApiKey;

using IdentityModel;

using Microsoft.Extensions.Logging;

namespace YourBrand.ApiKeys;

public class ApiKeyProvider : IApiKeyProvider
{
    private readonly ILogger<IApiKeyProvider> _logger;
    private readonly Client.IApiKeysClient _apiKeysClient;

    public ApiKeyProvider(ILogger<IApiKeyProvider> logger, Client.IApiKeysClient apiKeysClient)
    {
        _logger = logger;
        _apiKeysClient = apiKeysClient;
    }

    public async Task<IApiKey> ProvideAsync(string key)
    {
        try
        {
            var result = await _apiKeysClient.CheckApiKeyAsync(new Client.CheckApiKeyRequest()
            {
                ApiKey = key,
                RequestedResources = new [] { "foo" }
            });;

            if(result.Status == Client.ApiKeyStatus.Unauthorized)
            {
                throw new Exception("Invalid token");
            }

            return new ApiKey(key, "api", new List<Claim>
                {
                    new Claim(JwtClaimTypes.Subject, "api"),
                    new Claim(ClaimTypes.NameIdentifier, "api"),
                    new Claim(ClaimTypes.Role, "Administrator")
                });
        }
        catch (System.Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}
