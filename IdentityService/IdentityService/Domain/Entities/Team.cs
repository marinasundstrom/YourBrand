// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class Team 
{
    readonly HashSet<User> _members = new HashSet<User>();
    readonly HashSet<TeamMembership> _memberships = new HashSet<TeamMembership>();

    public string Id { get; set; }

    public string Name { get; set; }

    public Organization Organization { get; set; }

    public IReadOnlyCollection<User> Members => _members;

    public IReadOnlyCollection<TeamMembership> Memberships => _memberships;

    public void AddMember(User user)
    {

    }
}
