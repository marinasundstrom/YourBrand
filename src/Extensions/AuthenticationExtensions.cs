using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
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

                        // We have to hook the OnMessageReceived event in order to
                        // allow the JWT authentication handler to read the access
                        // token from the query string when a WebSocket or 
                        // Server-Sent Events request comes in.

                        // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
                        // due to a limitation in Browser APIs. We restrict it to only calls to the
                        // SignalR hub in this code.
                        // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                        // for more information about security considerations when using
                        // the query string to transmit the access token.

                        options.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];

                                // If the request is for our hub...
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) &&
                                    path.StartsWithSegments("/hubs"))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

        return services;
    }
}