using AspNetCore.Authentication.ApiKey;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Skynet.IdentityService;

public static class AuthSchemes
{
    public const string Default =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;
}