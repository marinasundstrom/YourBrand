using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> IncludeAll(this IQueryable<Product> source)
    {
        return source
                .IncludeBasics()
                .IncludeImages()
                .IncludeAttributes()
                .IncludeOptions()
                .Include(pv => pv.SubscriptionPlans);
    }

    public static IQueryable<Product> IncludeBasics(this IQueryable<Product> source)
    {
        return source
                .Include(pv => pv.Store)
                    .ThenInclude(pv => pv.Currency)
                .Include(pv => pv.Brand)
                .Include(pv => pv.Parent)
                    .ThenInclude(pv => pv!.Category)
                    .ThenInclude(pv => pv!.Parent)
                .Include(pv => pv.Brand)
                .Include(pv => pv.Category)
                    .ThenInclude(pv => pv!.Parent);
    }

    public static IQueryable<Product> IncludeImages(this IQueryable<Product> source)
    {
        return source
                .Include(pv => pv.Image)
                .Include(pv => pv.Images);
    }

    public static IQueryable<Product> IncludeAttributes(this IQueryable<Product> source)
    {
        return source
                .Include(pv => pv.ProductAttributes)
                    .ThenInclude(pv => pv.Value)
                .Include(pv => pv.ProductAttributes)
                    .ThenInclude(pv => pv.Attribute)
                    .ThenInclude(pv => pv.Values)
                .Include(pv => pv.ProductAttributes)
                    .ThenInclude(pv => pv.Attribute)
                    .ThenInclude(o => o.Group)
                .Include(pv => pv.ProductAttributes)
                    .ThenInclude(pv => pv.Attribute)
                    .ThenInclude(pv => pv.Values)
                .Include(pv => pv.ProductAttributes)
                    .ThenInclude(pv => pv.Value);
    }

    public static IQueryable<Product> IncludeOptions(this IQueryable<Product> source)
    {
        return source
                .Include(pv => pv.ProductOptions)
                    .ThenInclude(pv => pv.Option)
                    .ThenInclude(pv => pv.Group)
                .Include(pv => pv.ProductOptions)
                    .ThenInclude(pv => pv.Option)
                    .ThenInclude(pv => (pv as ChoiceOption)!.Values)
                .Include(pv => pv.Options)
                    .ThenInclude(pv => (pv as ChoiceOption)!.DefaultValue);
    }
}