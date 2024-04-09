using System.Security.Claims;

using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using IdentityModel;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityManagement.Client;

namespace YourBrand.IdentityManagement;

public sealed class CustomProfileService<TUser>(ILogger<DefaultProfileService> logger, UserManager<TUser> userManager, IUserClaimsPrincipalFactory<TUser> claimsFactory) : Duende.IdentityServer.AspNetIdentity.ProfileService<TUser>(userManager, claimsFactory)
    where TUser : IdentityUser
{
    public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        // add actor claim if needed
        if (context.Subject.GetAuthenticationMethod() == OidcConstants.GrantTypes.TokenExchange)
        {
            var act = context.Subject.FindFirst(JwtClaimTypes.Actor);
            if (act != null)
            {
                context.IssuedClaims.Add(act);
            }
        }

        await base.GetProfileDataAsync(context); //here the default implementation adds 
                                                 //claims from Subject (e.g. the cookie)
                                                 //and logs the result
        /*
        var user = await _userManager.GetUserAsync(context.Subject);

        var claims = new List<Claim>
        {
            new Claim("organization", "test")
        };

        context.IssuedClaims.AddRange(claims);
        */
    }

    /*
    public async Task IsActiveAsync(IsActiveContext context)
    {
        var user = await UserManager.GetUserAsync(context.Subject);

        context.IsActive = (user != null); // && user.;
    }
    */
}