using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public class ProductVariantsService(CatalogContext context)
{
    public async Task<IEnumerable<Product>> FindVariants(string organizationId, string productIdOrHandle, string? productVariantIdOrHandle, IDictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken)
    {
        bool isProductId = long.TryParse(productIdOrHandle, out var productId);

        var query = context.Products
            .InOrganization(organizationId)
            .AsSplitQuery()
            .AsNoTracking()
            .IncludeAll()
            .Include(x => x.Prices)
            .Include(x => x.Parent)
            .ThenInclude(x => x.Prices)
            .AsQueryable()
            .TagWith(nameof(FindVariants));

        query = isProductId ?
            query.Where(pv => pv.Parent!.Id == productId)
            : query.Where(pv => pv.Parent!.Handle == productIdOrHandle);

        if (productVariantIdOrHandle is not null)
        {
            bool isProductVariantId = long.TryParse(productVariantIdOrHandle, out var itemVariantId);

            query = isProductVariantId ?
                query.Where(pv => pv.Id == itemVariantId)
                : query.Where(pv => pv.Handle == productVariantIdOrHandle);
        }

        IEnumerable<Product> variants = await query
            .ToArrayAsync(cancellationToken);

        foreach (var selectedOption in selectedAttributeValues)
        {
            if (selectedOption.Value is null)
                continue;

            variants = variants.Where(x => x.ProductAttributes.Any(vv => vv.Attribute.Id == selectedOption.Key && vv.Value?.Id == selectedOption.Value));
        }

        return variants;
    }

    public async Task<IEnumerable<AttributeValue>> GetAvailableAttributeValues(string organizationId, string productIdOrHandle, string attributeId, IDictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken)
    {
        bool isProductId = long.TryParse(productIdOrHandle, out var productId);

        var query = context.Products
            .InOrganization(organizationId)
            .AsSplitQuery()
            .AsNoTracking()
            .Include(pv => pv.ProductAttributes)
            .ThenInclude(pv => pv.Attribute)
            .ThenInclude(pv => pv.Values)
            .Include(pv => pv.ProductAttributes)
            .ThenInclude(pv => pv.Value)
            .AsQueryable();

        query = isProductId ?
            query.Where(pv => pv.Parent!.Id == productId)
            : query.Where(pv => pv.Parent!.Handle == productIdOrHandle);

        IEnumerable<Product> variants = await query.ToArrayAsync(cancellationToken);

        foreach (var selectedAttribute in selectedAttributeValues)
        {
            if (selectedAttribute.Value is null)
                continue;

            variants = variants.Where(x => x.ProductAttributes.Any(vv => vv.Attribute.Id == selectedAttribute.Key && vv.Value?.Id == selectedAttribute.Value));
        }

        return variants
            .SelectMany(x => x.ProductAttributes)
            .Where(x => x.Attribute.Id == attributeId)
            .Select(x => x.Value!)
            .DistinctBy(x => x.Id);
    }
}