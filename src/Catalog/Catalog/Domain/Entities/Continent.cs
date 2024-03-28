namespace YourBrand.Catalog.Domain.Entities;

public class Continent
{
    protected Continent() { }

    public Continent(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
}