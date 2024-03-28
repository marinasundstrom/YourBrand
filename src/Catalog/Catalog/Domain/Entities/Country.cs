namespace YourBrand.Catalog.Domain.Entities;

public class Country
{
    protected Country() { }

    public Country(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? NativeName { get; set; }

    public string Capital { get; set; } = null!;

    public Continent Continent { get; set; } = null!;

    //public Currency Currency { get; set; } = null!;

    //public Language Language { get; set; } = null!;

    public List<Currency> Currencies { get; set; } = new List<Currency>();

    public List<Language> Languages { get; set; } = new List<Language>();

    public List<Region> Regions { get; set; } = new List<Region>();

    public List<CountryLanguage> CountryLanguages { get; set; } = new List<CountryLanguage>();

    public List<CountryCurrency> CountryCurrencies { get; set; } = new List<CountryCurrency>();
}

public class CountryLanguage
{
    public Country Country { get; set; } = null!;

    public Language Language { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string LanguageCode { get; set; } = null!;
}

public class CountryCurrency
{
    public Country Country { get; set; } = null!;

    public Currency Currency { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string CurrencyCode { get; set; } = null!;
}