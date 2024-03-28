namespace YourBrand.Catalog.Domain.Entities;

public class Language
{
    protected Language() { }

    public Language(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string NativeName { get; set; } = null!;

    public List<Country> Countries { get; set; } = new List<Country>();

    public List<CountryLanguage> CountryLanguages { get; set; } = new List<CountryLanguage>();
}