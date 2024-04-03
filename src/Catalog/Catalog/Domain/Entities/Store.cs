using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class Store : Entity<string>, IHasTenant
{
    readonly HashSet<Product> _products = new HashSet<Product>();

    protected Store() { }

    public Store(string name, string handle, Currency currency) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Handle = handle;
        Currency = currency;
    }

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public Currency Currency { get; set; } = null!;

    public CurrencyDisplayOptions CurrencyDisplayOptions { get; set; }

    public PricingOptions PricingOptions { get; set; }

    public IReadOnlyCollection<Product> Products => _products;

    public void AddProduct(Product product) => _products.Add(product);
}

public class CurrencyDisplayOptions
{
    public bool IncludeVatInSalesPrice { get; set; } = false;
    public int? RoundingDecimals { get; set; } = null;
}

public class PricingOptions
{
    public VatRate? DefaultVatRate { get; set; }

    public double? ProfitMarginRate { get; set; } = 0.2;

    public List<CategoryPricingOptions> CategoryPricingOptions { get; set; } = new List<CategoryPricingOptions>();
}

public class CategoryPricingOptions
{
    public string CategoryId { get; set; } = default!;

    public VatRate? DefaultVatRate { get; set; }

    public double ProfitMarginRate { get; set; }
}