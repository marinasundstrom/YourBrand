// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityService.Domain.Entities;

public class Role : IdentityRole<string>
{
    readonly HashSet<User> _users = new HashSet<User>();
    readonly HashSet<UserRole> _userRoles = new HashSet<UserRole>();

    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public IReadOnlyCollection<User> Users => _users;

    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;
}
