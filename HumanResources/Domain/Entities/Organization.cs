using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Events;

namespace YourBrand.HumanResources.Domain.Entities;

public class Organization : AuditableEntity
{
    private readonly HashSet<Team> _teams = new HashSet<Team>();
    private readonly HashSet<Person> _persons = new HashSet<Person>();

    private Organization() { }

    public Organization(string id, string name)
    {
        Id = id;
        Name = name;

        AddDomainEvent(new OrganizationCreated(Id));
    }

    public Organization(string name) : this(Guid.NewGuid().ToString(), name)
    {

    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string Currency { get; set; } 

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<Person> Persons => _persons;

    public void ChangeName(string name)
    {
        if(Name != name) 
        {
            Name = name;
        }
    }
}
