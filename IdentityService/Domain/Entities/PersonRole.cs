// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityService.Domain.Entities;

public class PersonRole : IdentityUserRole<string>
{
    public Person User { get; set; }

    public Role Role { get; set; }
}
