namespace YourBrand.Catalog.Domain.Entities;

public class Currency
{
    protected Currency() { }

    public Currency(string code, string name, string symbol)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
    }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Symbol { get; set; } = null!;

    public List<Country> Countries { get; set; } = new List<Country>();

    public List<CountryCurrency> CountryCurrencies { get; set; } = new List<CountryCurrency>();
}