using Duende.IdentityServer.Models;

using IdentityModel;

namespace YourBrand.IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
          new IdentityResource[]
          {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
                
                // let's include the role claim in the profile
                //new ProfileWithRoleIdentityResource(),
          };

    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
                // the api requires the role claim
                new ApiResource("myapi", "The Web Api", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "myapi" }
                },
                // the api requires the role claim
                new ApiResource("worker", "The Web Api", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "myapi" }
                },
                // the api requires the role claim
                new ApiResource("timereport", "The Web Api", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "myapi" }
                },
                 new ApiResource("catalogapi", "The Catalog API", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "catalogapi" }
                },
                new ApiResource("cartsapi", "The Carts API", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "cartsapi" }
                },
                new ApiResource("salesapi", "The Carts API", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role })
                {
                    Scopes = new string[] { "salesapi" }
                }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            // the api requires the role claim
            new ApiScope("myapi", "Access the api"),
            new ApiScope("catalogapi", "Access the Catalog API"),
            new ApiScope("cartsapi", "Access the Carts API"),
            new ApiScope("salesapi", "Access the Carts API")
        };

    public static IEnumerable<Duende.IdentityServer.Models.Client> Clients =>
            [
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "clientapp",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedCorsOrigins = { "https://localhost:5001" },
                AllowedScopes = { "openid", "profile", "email", "myapi", "catalogapi", "salesapi" },
                RedirectUris = { "https://localhost:5001/authentication/login-callback" },
                PostLogoutRedirectUris = { "https://localhost:5001/" },
                Enabled = true
            },
            new Duende.IdentityServer.Models.Client
            {
                ClientId = "storefront",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("foobar123!".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "profile", "email", "myapi", "catalogapi", "cartsapi", "salesapi" },
            }
            ];
}