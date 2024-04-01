using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record UpdateProductCategoryDetails(string IdOrPath, string Name, string Description) : IRequest<Result<ProductCategory>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductCategoryDetails, Result<ProductCategory>>
    {
        public async Task<Result<ProductCategory>> Handle(UpdateProductCategoryDetails request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrPath, out var id);

            var productCategory = isId ?
                await catalogContext.ProductCategories.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.ProductCategories.FirstOrDefaultAsync(product => product.Path == request.IdOrPath, cancellationToken);

            if (productCategory is null)
            {
                return Result.Failure<ProductCategory>(Errors.ProductCategoryNotFound);
            }

            productCategory.Name = request.Name;
            productCategory.Description = request.Description;

            await catalogContext.SaveChangesAsync(cancellationToken);

            productCategory = await catalogContext.ProductCategories
                .Include(x => x.Parent)
                .AsNoTracking()
                .FirstAsync(x => x.Id == productCategory.Id, cancellationToken);

            return Result.Success<ProductCategory>(productCategory.ToDto());
        }
    }
}