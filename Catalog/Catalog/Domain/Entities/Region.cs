namespace YourBrand.Catalog.Domain.Entities;

public class Region
{
    protected Region() { }

    public Region(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public int Id { get; set; }

    public Country Country { get; set; } = null!;

    public string? Code { get; set; } = null!;

    public string Name { get; set; } = null!;
}