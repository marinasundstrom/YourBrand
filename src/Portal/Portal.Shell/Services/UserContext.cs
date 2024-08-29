﻿using System.Data;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;

namespace YourBrand.Portal.Services;

public class UserContext(AuthenticationStateProvider authenticationStateProvider) : IUserContext
{
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;

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

    public async Task<string?> GetTenantId()
    {
        ClaimsPrincipal user = await GetUser();

#if DEBUG
        //Console.WriteLine("Claims: {0}", System.Text.Json.JsonSerializer.Serialize(user.Claims.Select(x => x.Type + " " + x.Value)));
#endif

        var tenantId = user?.FindFirst("tenant_id")?.Value;

#if DEBUG
        Console.WriteLine("Tenant Id: {0}", tenantId);
#endif

        return tenantId;
    }

    public async Task<IEnumerable<string>> GetRoles()
    {
        ClaimsPrincipal user = await GetUser();

        var roles = user?.FindAll("role").Select(x => x.Value);

#if DEBUG
        Console.WriteLine("Roles: {0}", roles == null ? null : string.Join(",", roles));
#endif

        return roles ?? Array.Empty<string>();
    }

    public async Task<bool> IsUserInRole(string role)
    {
        ClaimsPrincipal user = await GetUser();

        var roles = user?.FindAll("role").Select(x => x.Value);

#if DEBUG
        Console.WriteLine("Roles: {0}", roles == null ? null : string.Join(",", roles));
#endif

        return roles?.Contains(role) ?? false;
    }

    private async Task<ClaimsPrincipal> GetUser()
    {
        var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authenticationState.User;
        return user;
    }
}