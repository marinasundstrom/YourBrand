// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityService.Domain.Entities;

public class UserRole : IdentityUserRole<string>
{
    public Employee User { get; set; }

    public Role Role { get; set; }
}
