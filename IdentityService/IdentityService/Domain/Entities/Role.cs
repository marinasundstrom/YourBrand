// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityService.Domain.Entities;

public class Role : IdentityRole<string>
{
    readonly HashSet<Person> _persons = new HashSet<Person>();
    readonly HashSet<PersonRole> _personRoles = new HashSet<PersonRole>();

    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public IReadOnlyCollection<Person> Persons => _persons;

    public IReadOnlyCollection<PersonRole> PersonRoles => _personRoles;
}
