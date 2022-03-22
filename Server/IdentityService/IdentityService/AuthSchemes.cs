using AspNetCore.Authentication.ApiKey;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace YourBrand.IdentityService;

public static class AuthSchemes
{
    public const string Default =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;
}