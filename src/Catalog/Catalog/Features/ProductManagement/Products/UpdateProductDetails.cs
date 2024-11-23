using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductDetails(string OrganizationId, string IdOrHandle, string Name, string Description) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductDetails, Result>
    {
        public async Task<Result> Handle(UpdateProductDetails request, CancellationToken cancellationToken)
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

            product.Name = request.Name;
            product.Description = request.Description;

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductDetailsUpdated
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description
            });

            return Result.Success;
        }
    }
}