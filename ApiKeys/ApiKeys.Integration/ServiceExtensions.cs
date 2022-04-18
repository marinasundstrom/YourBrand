
using AspNetCore.Authentication.ApiKey;

using YourBrand.ApiKeys.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.ApiKeys;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services, string url, Action<IHttpClientBuilder>? builder = null)
    {
        services.AddApiKeyClient((sp, http) =>
        {
            http.BaseAddress = new Uri(url);
        }, builder);

        services.AddAuthentication(ApiKeyDefaults.AuthenticationScheme)

            // The below AddApiKeyInHeaderOrQueryParams without type parameter will require options.Events.OnValidateKey delegete to be set.
            //.AddApiKeyInHeaderOrQueryParams(options =>

            // The below AddApiKeyInHeaderOrQueryParams with type parameter will add the ApiKeyProvider to the dependency container. 
            .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
            {
                options.Realm = "Identity Service API";
                options.KeyName = "X-API-KEY";
            });

        return services;
    }
}

