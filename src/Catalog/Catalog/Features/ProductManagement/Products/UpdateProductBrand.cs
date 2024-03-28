using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductBrand(string IdOrHandle, long BrandId) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductBrand, Result>
    {
        public async Task<Result> Handle(UpdateProductBrand request, CancellationToken cancellationToken)
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

            var brand = await catalogContext.Brands
                .FirstOrDefaultAsync(brand => brand.Id == request.BrandId, cancellationToken);

            if (brand is null)
            {
                return Result.Failure(Errors.BrandNotFound);
            }

            product.Brand = brand;

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}