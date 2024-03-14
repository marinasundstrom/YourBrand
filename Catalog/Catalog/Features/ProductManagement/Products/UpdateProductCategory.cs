using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductCategory(string IdOrHandle, long ProductCategoryId) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductCategory, Result>
    {
        public async Task<Result> Handle(UpdateProductCategory request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var query = catalogContext.Products
                .Include(product => product.Category)
                .ThenInclude(category => category!.Parent);

            var product = isId ?
                await query.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await query.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            var newCategory = await catalogContext.ProductCategories
                .Include(productCategory => productCategory.Parent)
                .Include(productCategory => productCategory.Products)
                .FirstOrDefaultAsync(productCategory => productCategory.Id == request.ProductCategoryId, cancellationToken);

            if (newCategory is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            product.Category.RemoveProduct(product);

            newCategory.AddProduct(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}