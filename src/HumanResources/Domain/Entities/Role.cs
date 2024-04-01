namespace YourBrand.HumanResources.Domain.Entities;

public class Role
{
    readonly HashSet<Person> _persons = new HashSet<Person>();
    readonly HashSet<PersonRole> _personRoles = new HashSet<PersonRole>();

    internal Role()
    {

    }

    public Role(string name)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
    }

    public string Id { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<Person> Persons => _persons;

    public IReadOnlyCollection<PersonRole> PersonRoles => _personRoles;
}