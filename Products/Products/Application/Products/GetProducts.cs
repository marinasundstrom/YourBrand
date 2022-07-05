using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products;

public record GetProducts(bool IncludeUnlisted = false, string? GroupId = null, int Page = 10, int PageSize = 10) : IRequest<ApiProductsResult>
{
    public class Handler : IRequestHandler<GetProducts, ApiProductsResult>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiProductsResult> Handle(GetProducts request, CancellationToken cancellationToken)
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

            var totalCount = await query.CountAsync();

            var products = await query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new ApiProductsResult(products.Select(x => new ApiProduct(x.Id, x.Name, x.Description, x.Group == null ? null : new ApiProductGroup(x.Group.Id, x.Group.Name, x.Group.Description, x.Group?.Parent?.Id),
                x.SKU, GetImageUrl(x.Image), x.Price, x.HasVariants, x.Visibility == Domain.Enums.ProductVisibility.Listed ? ProductVisibility.Listed : ProductVisibility.Unlisted)),
                totalCount);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
