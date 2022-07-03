using YourBrand.HumanResources.Domain.Common;

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

    public void AddMember(Person person)
    {
        _members.Add(person);
    }

    public void RemoveMember(Person person)
    {
        _members.Remove(person);
    }
}
