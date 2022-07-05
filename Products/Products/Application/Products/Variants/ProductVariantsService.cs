using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Variants;

public class ProductVariantsService 
{
    private readonly IProductsContext _context;

    public ProductVariantsService(IProductsContext context)
    {
        _context = context;
    }

    public async Task<ProductVariant?> FindVariantCore(string productId, string? productVariantId, IDictionary<string, string?> selectedOptions)
    {
        var query = _context.ProductVariants
            .AsSplitQuery()
            .AsNoTracking()
            .Include(pv => pv.Product)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Attribute)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Value)
            .Where(pv => pv.Product.Id == productId)
            .AsQueryable();

        if (productVariantId is not null)
        {
            query = query.Where(pv => pv.Id != productVariantId);
        }

        IEnumerable<ProductVariant> variants = await query
            .ToArrayAsync();

        foreach (var selectedOption in selectedOptions)
        {
            if (selectedOption.Value is null)
                continue;

            variants = variants.Where(x => x.Values.Any(vv => vv.Attribute.Id == selectedOption.Key && vv.Value.Id == selectedOption.Value));
        }

        return variants.SingleOrDefault((ProductVariant?)null);
    }
}