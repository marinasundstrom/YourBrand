namespace YourBrand.HumanResources.Domain.Entities;

public class Role
{
    readonly HashSet<Person> _persons = new HashSet<Person>();
    readonly HashSet<PersonRole> _personRoles = new HashSet<PersonRole>();

    public Role()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public IReadOnlyCollection<Person> Persons => _persons;

    public IReadOnlyCollection<PersonRole> PersonRoles => _personRoles;
}
