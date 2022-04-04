using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace YourBrand.Portal.Shared;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
        : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[] { "https://localhost/api", "https://localhost/worker", "https://localhost/timereport", "https://localhost/showroom", "https://identity.local" },
            scopes: new[] { "myapi" });
    }
}