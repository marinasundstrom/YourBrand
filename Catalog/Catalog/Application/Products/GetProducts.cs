using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Common.Models;
using YourBrand.Catalog.Application.Products.Groups;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products;

public record GetProducts(bool IncludeUnlisted = false, string? GroupId = null, int Page = 10, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<ProductDto>>
{
    public class Handler : IRequestHandler<GetProducts, ItemsResult<ProductDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<ProductDto>> Handle(GetProducts request, CancellationToken cancellationToken)
        {
            var query = _context.Products
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .AsQueryable();

            if (!request.IncludeUnlisted)
            {
                query = query.Where(x => x.Visibility == Domain.Enums.ProductVisibility.Listed);
            }

            if (request.GroupId is not null)
            {
                query = query.Where(x => x.Group.Id == request.GroupId);
            }

            if (request.SearchString is not null)
            {
                query = query.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await query.CountAsync();

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Catalog.Application.SortDirection.Descending : Catalog.Application.SortDirection.Ascending);
            }

            var products = await query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new ItemsResult<ProductDto>(products.Select(x => new ProductDto(x.Id, x.Name, x.Description, x.Group == null ? null : new ProductGroupDto(x.Group.Id, x.Group.Name, x.Group.Description, x.Group?.Parent?.Id),
                x.SKU, GetImageUrl(x.Image), x.Price, x.HasVariants, x.Visibility == Domain.Enums.ProductVisibility.Listed ? ProductVisibility.Listed : ProductVisibility.Unlisted)),
                totalCount);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
