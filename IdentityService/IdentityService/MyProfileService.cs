namespace YourBrand.IdentityService;

using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using IdentityModel;

public class MyProfileService : IProfileService
{
    public MyProfileService()
    { }

    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        //get name claims from ClaimsPrincipal 
        var nameclaims = context.Subject.FindAll(JwtClaimTypes.Name);

        //add your name claims 
        context.IssuedClaims.AddRange(nameclaims);

        //get role claims from ClaimsPrincipal 
        var roleClaims = context.Subject.FindAll(JwtClaimTypes.Role);

        //add your role claims 
        context.IssuedClaims.AddRange(roleClaims);

        //get tenant claims from ClaimsPrincipal 
        var tenantClaim = context.Subject.FindAll("organizationId");

        //add your tenant claims 
        context.IssuedClaims.AddRange(tenantClaim);

        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        // await base.IsActiveAsync(context);
        return Task.CompletedTask;
    }
}