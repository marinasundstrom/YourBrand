using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using YourBrand.ApiKeys;

namespace YourBrand.Messenger.Authentication;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuthWithJwt(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://localhost:5040";
                        options.Audience = "myapi";

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            NameClaimType = "name"
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                // Add the access_token as a claim, as we may actually need it
                                var accessToken = context.SecurityToken as JwtSecurityToken;
                                if (accessToken != null)
                                {
                                    ClaimsIdentity? identity = context?.Principal?.Identity as ClaimsIdentity;
                                    if (identity != null)
                                    {
                                        identity.AddClaim(new Claim("access_token", accessToken.RawData));
                                    }
                                }

                                return Task.CompletedTask;
                            }
                        };

                        //options.TokenValidationParameters.ValidateAudience = false;

                        //options.Audience = "openid";

                        //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    });

        return services;
    }

    public static IServiceCollection AddAuthWithApiKey(this IServiceCollection services)
    {
        services.AddApiKeyAuthentication("https://localhost:5174/apikeys/");

        return services;
    }
}