namespace YourBrand.HumanResources.Domain.Entities;

public class Department(string name, string description)
{
    readonly HashSet<Person> _persons = new HashSet<Person>();

    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public string Name { get; private set; } = name;

    public string? Description { get; private set; } = description;

    public Organization Organization { get; private set; }

    public IReadOnlyCollection<Person> Persons => _persons;
}