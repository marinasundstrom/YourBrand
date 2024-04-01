using System.Security.Claims;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityManagement;

public sealed class CustomProfileService : IProfileService
{
    private readonly UserManager<Domain.Entities.User> _userManager;

    public CustomProfileService(UserManager<Domain.Entities.User> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);



        var claims = new List<Claim>
        {
            new Claim("organization", "test")
        };

        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        context.IsActive = (user != null); // && user.;
    }
}