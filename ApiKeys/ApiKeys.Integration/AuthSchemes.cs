
using AspNetCore.Authentication.ApiKey;

namespace YourBrand.ApiKeys;

public static class AuthSchemes
{
    public const string Default =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;
}