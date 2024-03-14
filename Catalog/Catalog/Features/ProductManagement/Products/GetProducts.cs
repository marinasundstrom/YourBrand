using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record GetProducts(string? StoreId = null, string? BrandIdOrHandle = null, bool IncludeUnlisted = false, bool GroupProducts = true, string? ProductCategoryIdOrPath = null, string? SearchTerm = null, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<ProductDto>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProducts, PagedResult<ProductDto>>
    {
        public async Task<PagedResult<ProductDto>> Handle(GetProducts request, CancellationToken cancellationToken)
        {
            var query = catalogContext.Products
                        .Where(x => x.Category != null)
                        .IncludeAll()
                        .AsSplitQuery()
                        .AsNoTracking()
                        .AsQueryable()
                        .TagWith(nameof(GetProducts));

            if (request.StoreId is not null)
            {
                query = query.Where(x => x.StoreId == request.StoreId);
            }

            if (request.BrandIdOrHandle is not null)
            {
                bool isBrandId = long.TryParse(request.BrandIdOrHandle, out var brandId);

                query = isBrandId ?
                    query.Where(pv => pv.BrandId == brandId)
                    : query.Where(pv => pv.Brand!.Handle == request.BrandIdOrHandle);
            }

            if (!request.IncludeUnlisted)
            {
                query = query.Where(x => x.ListingState == Domain.Enums.ProductListingState.Listed);
            }

            if (!string.IsNullOrEmpty(request.ProductCategoryIdOrPath))
            {
                bool isProductCategoryId = long.TryParse(request.ProductCategoryIdOrPath, out var categoryId);

                query = isProductCategoryId
                            ? query.Where(x => x.Category!.Id == categoryId)
                            : query.Where(x =>
                                x.Category!.Path.StartsWith(request.ProductCategoryIdOrPath));
            }

            if (request.GroupProducts)
            {
                query = query.Where(x => x.ParentId == null);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var t = $"%{request.SearchTerm}%";
                query = query.Where(x => EF.Functions.Like(x.Name, t) || EF.Functions.Like(x.Description, t));
            }

            var total = await query.CountAsync(cancellationToken);

            if (request.SortBy is null || request.SortDirection is null)
            {
                query = query.OrderBy(x => x.Name);
            }
            else
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }

            var products = await query
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductDto>(products.Select(x => x.ToDto()), total);
        }
    }
}