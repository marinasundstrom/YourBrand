// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

namespace YourBrand.IdentityService.Domain.Entities;

public class Team 
{
    public string Id { get; set; }

    public string Name { get; set; }

    public Organization Organization { get; set; }

    public List<User> Members { get; } = new List<User>();

    public List<TeamMembership> Memberships { get; set; } = new List<TeamMembership>();
}
