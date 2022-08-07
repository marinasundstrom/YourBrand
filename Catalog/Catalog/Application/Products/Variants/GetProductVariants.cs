using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Common.Models;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record GetProductVariants(string ProductId,  int Page = 10, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProductVariantDto>>
{
    public class Handler : IRequestHandler<GetProductVariants, ItemsResult<ProductVariantDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProductVariantDto>> Handle(GetProductVariants request, CancellationToken cancellationToken)
        {
            var query = _context.ProductVariants
                .Where(pv => pv.Product.Id == request.ProductId)
                .OrderBy(x => x.Id)
                .AsSplitQuery()
                .AsNoTracking()
                .AsQueryable();

            if (request.SearchString is not null)
            {
                query = query.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await query.CountAsync();

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Catalog.Application.SortDirection.Descending : Catalog.Application.SortDirection.Ascending);
            }

            var variants = await query
                .Include(pv => pv.Product)
                .Include(pv => pv.AttributeValues)
                .ThenInclude(pv => pv.Attribute)
                .Include(pv => pv.AttributeValues)
                .ThenInclude(pv => pv.Value)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new ItemsResult<ProductVariantDto>(variants.Select(x => new ProductVariantDto(x.Id, x.Name, x.Description, x.SKU, GetImageUrl(x.Image), x.Price,
                x.AttributeValues.Select(x => x.ToDto()))),
                totalCount);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
