using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class UserRole : IdentityUserRole<string>
{
    public User User { get; set; }

    public Role Role { get; set; }
}