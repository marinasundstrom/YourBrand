using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public sealed record DeleteProductCategory(string IdOrPath) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<DeleteProductCategory, Result>
    {
        public async Task<Result> Handle(DeleteProductCategory request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrPath, out var id);

            var product = isId ?
                await catalogContext.ProductCategories.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.ProductCategories.FirstOrDefaultAsync(product => product.Path == request.IdOrPath, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductCategoryNotFound);
            }

            catalogContext.ProductCategories.Remove(product);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}