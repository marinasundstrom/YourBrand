
using System.Security.Claims;

using AspNetCore.Authentication.ApiKey;

using IdentityModel;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace YourBrand.ApiKeys;

public class ApiKeyProvider(ILogger<IApiKeyProvider> logger, IConfiguration configuration, Client.IApiKeysClient apiKeysClient) : IApiKeyProvider
{
    public async Task<IApiKey> ProvideAsync(string key)
    {
        try
        {
            string secret = configuration["ApiKeyService:Secret"];
            string[] requestedResources = configuration.GetSection("ApiKeyService:RequestedResources").Get<string[]>();

            var result = await apiKeysClient.CheckApiKeyAsync(secret, new Client.CheckApiKeyRequest()
            {
                ApiKey = key,
                RequestedResources = requestedResources
            });

            //Console.WriteLine(result.Status);

            if (result.Status != Client.ApiKeyAuthCode.Authorized)
            {
                throw new Exception("Invalid token");
            }

            return new ApiKey(key, "api", new List<Claim>
                {
                    // TODO: Fix

                    new Claim(JwtClaimTypes.Subject, "api"), // AppId
                    new Claim(ClaimTypes.NameIdentifier, "api"), // AppId
                    new Claim(ClaimTypes.Name, result.Application.Name),
                    new Claim(ClaimTypes.Role, "Administrator"), // Remove
                    new Claim(ClaimTypes.Role, "API")

                    // new Claim("AppId", result.Application.Id),
                    // new Claim(ClaimTypes.NameIdentifier, result.Application.Id),
                    // new Claim(ClaimTypes.Name, result.Application.Name),
                    // new Claim(ClaimTypes.Role, "Administrator") // "App"
                });
        }
        catch (System.Exception exception)
        {
            logger.LogError(exception, exception.Message);
            throw;
        }
    }
}