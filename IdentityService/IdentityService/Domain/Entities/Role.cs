// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityService.Domain.Entities;

public class Role : IdentityRole<string>
{
    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public List<User> Users { get; } = new List<User>();

    public List<UserRole> UserRoles { get; } = new List<UserRole>();
}
