using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductHandle(string OrganizationId, string IdOrHandle, string Handle) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductHandle, Result>
    {
        public async Task<Result> Handle(UpdateProductHandle request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            var p = product;

            var handleInUse = await catalogContext.Products.AnyAsync(product => product.Id != p.Id && product.Handle == request.Handle, cancellationToken);

            if (handleInUse)
            {
                return Result.Failure(Errors.HandleAlreadyTaken);
            }

            product.Handle = request.Handle;

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductHandleUpdated
            {
                ProductId = product.Id,
                Handle = product.Handle
            });


            return Result.Success;
        }
    }
}