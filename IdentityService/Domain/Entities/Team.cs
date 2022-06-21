// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using YourBrand.IdentityService.Domain.Common;

namespace YourBrand.IdentityService.Domain.Entities;

public class Team : AuditableEntity
{
    readonly HashSet<Person> _members = new HashSet<Person>();
    readonly HashSet<TeamMembership> _memberships = new HashSet<TeamMembership>();

    private Team()
    {
    }

    public Team(string name, string? description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public void UpdateName(string value)
    {
        Name = value;
    }

    public string? Description { get; private set; }

    public void UpdateDescription(string value)
    {
        Description = value;
    }

    public Organization Organization { get; private set; }

    public IReadOnlyCollection<Person> Members => _members;

    public IReadOnlyCollection<TeamMembership> Memberships => _memberships;

    public void AddMember(Person user)
    {
        _members.Add(user);
    }

    public void RemoveMember(Person user)
    {
        _members.Remove(user);
    }
}
