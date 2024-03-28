using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record GetProductCategoryTree(string? StoreId, string? RootNodeIdOrPath) : IRequest<Result<ProductCategoryTreeRootDto>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProductCategoryTree, Result<ProductCategoryTreeRootDto>>
    {
        public async Task<Result<ProductCategoryTreeRootDto>> Handle(GetProductCategoryTree request, CancellationToken cancellationToken)
        {
            var query = catalogContext.ProductCategories
            .Include(x => x.Parent)
            .ThenInclude(x => x!.Parent)
            .Include(x => x.SubCategories.OrderBy(x => x.Name))
            .Where(x => x.Parent == null)
            .OrderBy(x => x.Name)
            .AsSingleQuery()
            .AsNoTracking();

            if (request.StoreId is not null)
            {
                query = query.Where(x => x.StoreId == request.StoreId);
            }

            if (!string.IsNullOrEmpty(request.RootNodeIdOrPath))
            {
                var isId = int.TryParse(request.RootNodeIdOrPath, out var id);

                query = isId ?
                        query.Where(category => category.Id == id)
                        : query.Where(category => category.Path == request.RootNodeIdOrPath);
            }

            var productCategories = await query
                .ToArrayAsync(cancellationToken);

            var root = new ProductCategoryTreeRootDto(productCategories.Select(x => x.ToProductCategoryTreeNodeDto()), productCategories.Sum(x => x.ProductsCount));

            return Result.Success(root);
        }
    }
}