namespace YourBrand.HumanResources.Domain.Entities;

public class Department
{
    readonly HashSet<Person> _persons = new HashSet<Person>();

    public Department(string name, string description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public string Id { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public Organization Organization { get; private set; }

    public IReadOnlyCollection<Person> Persons => _persons;
}