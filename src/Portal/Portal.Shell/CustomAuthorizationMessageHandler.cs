using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace YourBrand.Portal;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation)
        : base(provider, navigation)
    {
        ConfigureHandler(
            authorizedUrls: [
                "https://localhost:5174"
            ],
            scopes: ["myapi"]);
    }
}