using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductImage : Entity<string>, IHasTenant, IHasOrganization, IHasStore
{
    public ProductImage() { }

    public ProductImage(string title, string text, string url)
    {
        Id = Guid.NewGuid().ToString();
        Title = title;
        Text = text;
        Url = url;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Store? Store { get; set; }

    public string? StoreId { get; set; }

    public Product? Product { get; set; } = default!;

    public int? ProductId { get; set; }

    public string Title { get; set; }

    public string? Text { get; set; }

    public string Url { get; set; }
}