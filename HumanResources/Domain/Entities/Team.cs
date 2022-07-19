using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Events;

namespace YourBrand.HumanResources.Domain.Entities;

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

        AddDomainEvent(new TeamCreated(Id));
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

    public Organization Organization { get; set; }

    public IReadOnlyCollection<Person> Members => _members;

    public IReadOnlyCollection<TeamMembership> Memberships => _memberships;

    public TeamMembership AddMember(Person person)
    {
        var membership = new TeamMembership(person);
        _memberships.Add(membership);
        AddDomainEvent(new TeamMemberAdded(Id, person.Id));
        return membership;
    }

    public void RemoveMember(Person person)
    {
        
        AddDomainEvent(new TeamMemberRemoved(Id, person.Id));
        _members.Remove(person);
    }
}
