using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class Role : IdentityRole<string>
{
    readonly HashSet<User> _users = new HashSet<User>();
    readonly HashSet<UserRole> _userRoles = new HashSet<UserRole>();

    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public Role(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public IReadOnlyCollection<User> Users => _users;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
}