using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record GetProductVariants(string OrganizationId, string ProductIdOrHandle, int Page = 1, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<Catalog.Features.ProductManagement.Products.ProductDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetProductVariants, PagedResult<Catalog.Features.ProductManagement.Products.ProductDto>>
    {
        public async Task<PagedResult<Catalog.Features.ProductManagement.Products.ProductDto>> Handle(GetProductVariants request, CancellationToken cancellationToken)
        {
            bool isProductId = long.TryParse(request.ProductIdOrHandle, out var productId);

            var query = context.Products
                .InOrganization(request.OrganizationId)
                .AsQueryable();

            query = isProductId ?
                query.Where(pv => pv.Parent!.Id == productId)
                : query.Where(pv => pv.Parent!.Handle == request.ProductIdOrHandle);

            query = query
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
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }

            var variants = await query
                .IncludeAll()
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new PagedResult<ProductDto>(variants.Select(item => item.ToDto()), totalCount);
        }
        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}