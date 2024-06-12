using Microsoft.AspNetCore.Components.Authorization;

namespace ChatApp.Services;

public interface ICurrentUserService 
{
    Task<string> GetUserIdAsync();
    Task<bool> IsInRoleAsync(string role);
}

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public CurrentUserService(AuthenticationStateProvider authenticationStateProvider)
    {
        this.authenticationStateProvider = authenticationStateProvider;
    }

    public async Task<string> GetUserIdAsync()
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User?.FindFirst("sub")?.Value!;
    }

    public async Task<bool> IsInRoleAsync(string role)
    {
        var authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
        return authenticationState.User.IsInRole(role);
    }
}