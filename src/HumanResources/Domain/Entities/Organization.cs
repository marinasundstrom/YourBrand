using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Events;
using YourBrand.Tenancy;

namespace YourBrand.HumanResources.Domain.Entities;

public class Organization : AuditableEntity<string>
{
    private readonly HashSet<Team> _teams = new HashSet<Team>();
    private readonly HashSet<Person> _persons = new HashSet<Person>();

    private Organization() { }

    public Organization(string id, string name, string friendlyName) : base(id)
    {
        Name = name;
        FriendlyName = friendlyName;

        AddDomainEvent(new OrganizationCreated(Id));
    }

    public Organization(string name, string friendlyName) : this(Guid.NewGuid().ToString(), name, friendlyName)
    {

    }

    public string Name { get; private set; }

    public string FriendlyName { get; private set; }

    public string Currency { get; set; } = "SEK";

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<Person> Persons => _persons;

    public void ChangeName(string name)
    {
        if (Name != name)
        {
            Name = name;
        }
    }
}