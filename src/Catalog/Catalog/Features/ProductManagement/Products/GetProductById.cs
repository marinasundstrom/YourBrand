using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record GetProductById(string IdOrHandle) : IRequest<Result<ProductDto>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProductById, Result<ProductDto>>
    {
        public async Task<Result<ProductDto>> Handle(GetProductById request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var query = catalogContext.Products
                .IncludeAll()
                .AsSplitQuery()
                .AsQueryable();

            var product = isId ?
                await query.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await query.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            var price = product.GetTotalOptionsPrice();

            Console.WriteLine(price);

            return product is null
                ? Result.Failure<ProductDto>(Errors.ProductNotFound)
                : Result.Success(product.ToDto());
        }
    }
}