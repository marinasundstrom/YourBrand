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
            authorizedUrls: new[] { "https://localhost/api", "https://localhost/api/worker", "https://localhost/api/timereport", "https://localhost/api/showroom", "https://localhost/api/humanresources", "https://localhost/api/warehouse", "https://localhost/api/customers", "https://identity.local" },
            scopes: new[] { "myapi" });
    }
}