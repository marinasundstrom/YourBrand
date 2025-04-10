﻿using Duende.IdentityServer.Models;

using IdentityModel;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("ssn", ["ssn"]),
            new IdentityResource("customer_id", ["customer_id"]),
        ];

    public static IEnumerable<ApiResource> ApiResources =>
        [
                new ApiResource("storefrontapi", "The StoreFront API", new[] { JwtClaimTypes.Name, JwtClaimTypes.PreferredUserName, JwtClaimTypes.Email, JwtClaimTypes.Role, "ssn", "customer_id" })
                {
                    Scopes = ["storefrontapi"]
                }
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("storefrontapi", "Access the StoreFront API", ["ssn", "customer_id"]),
        ];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "store",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                AllowedCorsOrigins = { "https://localhost:7188" },
                AllowedScopes = { "openid", "profile", "storefrontapi" },
                RedirectUris = { "https://localhost:7188/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:7188/signout-callback-oidc" },
                AllowOfflineAccess = true,
                Enabled = true
            },
            /*
            new Client
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
                AllowedScopes = { "profile", "email", "storefrontapi" },
            }
            */
        ];
}