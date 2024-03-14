using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Identity.Web;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace YourBrand.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var isDev = configuration["ASPNETCORE_ENVIRONMENT"] == "Development";

        if (isDev)
        {
            return AddAuthentication_IdentityServer(services, configuration);
        }

        return AddAuthentication_Entra(services, configuration);
    }

    public static IServiceCollection AddAuthentication_Entra(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

        return services;
    }

    public static IServiceCollection AddAuthentication_IdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = configuration.GetValue<string>("Local:Authority");
                        options.Audience = configuration.GetValue<string>("Local:Audience");

                        Console.WriteLine(options.Authority);
                        Console.WriteLine("Audience: " + options.Audience);

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name",
                            ValidateAudience = false,
                            ValidTypes = new[] { "at+jwt" }
                        };
                    });

        return services;
    }
}