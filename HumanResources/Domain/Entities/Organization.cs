using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class Organization : AuditableEntity
{
    private readonly HashSet<Team> _teams = new HashSet<Team>();
    private readonly HashSet<Person> _persons = new HashSet<Person>();

    private Organization() { }

    public Organization(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;

        AddDomainEvent(new OrganizationCreated(Id));
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string Currency { get; set; } 

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<Person> Persons => _persons;
}
