using Duende.IdentityServer.Models;

using IdentityModel;

namespace IdentityService;

public class ProfileWithRoleIdentityResource : IdentityResources.Profile
{
    public ProfileWithRoleIdentityResource()
    {
        this.UserClaims.Add(JwtClaimTypes.PreferredUserName);
        this.UserClaims.Add(JwtClaimTypes.Role);
    }
}