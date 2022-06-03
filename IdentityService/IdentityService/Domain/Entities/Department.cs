// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class Department 
{
    readonly HashSet<User> _users = new HashSet<User>();

    public string Id { get; set; }

    public string Name { get; set; }

    public Organization Organization { get; set; }

    public IReadOnlyCollection<User> Users => _users;
}
