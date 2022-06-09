using System;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using YourBrand.Portal.Services;

namespace YourBrand.Portal.Shared.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public CurrentUserService(AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string?> GetUserId()
    {
        ClaimsPrincipal user = await GetUser();

#if DEBUG
        //Console.WriteLine("Claims: {0}", System.Text.Json.JsonSerializer.Serialize(user.Claims.Select(x => x.Type + " " + x.Value)));
#endif

        var name = user?.FindFirst("sub")?.Value;

#if DEBUG
        Console.WriteLine("User Id: {0}", name);
#endif

        return name;
    }

    public async Task<bool> IsUserInRole(string role)
    {
        ClaimsPrincipal user = await GetUser();

        var actualRole = user?.FindFirst("role")?.Value;

#if DEBUG
        Console.WriteLine("Role: {0}", actualRole);
#endif

        return actualRole == role;
    }

    private async Task<ClaimsPrincipal> GetUser()
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authenticationState.User;
        return user;
    }
}

