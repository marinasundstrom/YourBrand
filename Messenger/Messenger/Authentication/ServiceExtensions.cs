using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using AspNetCore.Authentication.ApiKey;

using YourBrand.ApiKeys;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace YourBrand.Messenger.Authentication;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuthWithJwt(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://identity.local";
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
        services.AddApiKeyAuthentication("https://localhost/apikeys/");

        return services;
    }
}

