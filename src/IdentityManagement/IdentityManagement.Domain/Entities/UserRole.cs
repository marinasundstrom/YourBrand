using Microsoft.AspNetCore.Identity;

using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class UserRole : IdentityUserRole<string>, IHasTenant
{
    public Tenant Tenant { get; set; }
    
    public TenantId TenantId { get; set; }

    public User User { get; set; }

    public Role Role { get; set; }
}