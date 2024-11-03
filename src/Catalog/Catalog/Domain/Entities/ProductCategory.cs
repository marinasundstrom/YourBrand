using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public sealed class ProductCategory : Entity<int>, IHasTenant, IHasOrganization, IHasStore
{
    private readonly HashSet<Product> _products = new HashSet<Product>();
    private readonly HashSet<ProductCategory> _subCategories = new HashSet<ProductCategory>();

    public ProductCategory() { }

    public ProductCategory(int id) : base(id)
    {

    }

    public ProductCategory(string name)
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Store Store { get; set; }

    public string StoreId { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public ProductCategory? Parent { get; set; }

    public int? ParentId { get; set; }

    public bool CanAddProducts { get; set; }

    public IReadOnlyCollection<Product> Products => _products;

    public long ProductsCount { get; set; }

    public void AddProduct(Product product)
    {
        if (!CanAddProducts)
        {
            throw new InvalidOperationException("Can not add products.");
        }

        product.OrganizationId = OrganizationId;
        product.Store = Store;
        product.Category = this;
        _products.Add(product);

        IncrementProductsCount();
    }

    public void RemoveProduct(Product product)
    {
        product.Category = null;
        _products.Remove(product);

        DecrementProductsCount();
    }

    public IReadOnlyCollection<ProductCategory> SubCategories => _subCategories;

    public void AddSubCategory(ProductCategory productCategory)
    {
        productCategory.Parent = this;
        productCategory.Path = $"{(Path is null ? Handle : Path)}/{productCategory.Handle}";
        _subCategories.Add(productCategory);
    }

    public void RemoveSubCategory(ProductCategory productCategory)
    {
        productCategory.Parent = null;
        _subCategories.Remove(productCategory);
    }

    internal void IncrementProductsCount()
    {
        ProductsCount++;

        Parent?.IncrementProductsCount();
    }

    private void DecrementProductsCount()
    {
        ProductsCount--;

        Parent?.DecrementProductsCount();
    }

    public string Handle { get; set; } = default!;

    public string Path { get; set; } = default!;

    public List<Attribute> Attributes { get; } = new List<Attribute>();

    public List<Option> Options { get; } = new List<Option>();
}