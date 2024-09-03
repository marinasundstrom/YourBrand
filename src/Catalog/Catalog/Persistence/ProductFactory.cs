using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Domain.Enums;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence;

public class ProductFactory(CatalogContext catalogContext, ITenantContext tenantContext)
{
    public async Task<Product> CreateProductAsync(string organizationId, ProductOptions options, CancellationToken cancellationToken = default)
    {
        long productId = 1;

        try
        {
            productId = await catalogContext.Products
                .Where(x => x.OrganizationId == organizationId)
                .MaxAsync(x => x.Id, cancellationToken) + 1;
        }
        catch { }

        var product = new Product(organizationId, productId, options.Name);

        product.Handle = options.Handle;

        product.Headline = options.Headline;

        product.Description = options.Description!;

        product.VatRate = options.VatRate;

        product.SetPrice(options.Price);

        if(options.DiscountPrice is not null) 
        {
            product.SetDiscountPrice(options.DiscountPrice.GetValueOrDefault());
        }

        product.StoreId = options.StoreId;

        product.BrandId = options.BrandId;

        product.HasVariants = options.HasVariants;

        product.ListingState = options.ListingState;

        product.ImageId = options.ImageId;

        product.Gtin = options.Gtin;

        product.VatRateId = (await catalogContext.VatRates.FirstOrDefaultAsync(x => x.Rate == product.VatRate))?.Id;

        catalogContext.Products.Add(product);

        await catalogContext.SaveChangesAsync(cancellationToken);

        return product;
    }
}

public class ProductOptions 
{
    public string Name { get; set;}

    public string Handle { get; set; }
    
    public string? Headline { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPrice { get; set; }

    public string? StoreId { get; set; }

    public int? BrandId { get; set; }

    public bool HasVariants { get; set; }

    public ProductListingState ListingState { get; set; }

    public string? ImageId { get; set; }

    public string Gtin { get; set; }

    public double VatRate { get; set; }
}