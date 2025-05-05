using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductVatRate(string OrganizationId, string IdOrHandle, int? VatRateId) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductVatRate, Result>
    {
        public async Task<Result> Handle(UpdateProductVatRate request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            if (request.VatRateId is not null)
            {
                var vatRate = await catalogContext.VatRates
                    .FirstOrDefaultAsync(x => x.Id == request.VatRateId, cancellationToken);

                if (vatRate is null)
                {
                    return Result.Failure(Errors.VatRateNotFound);
                }

                product.VatRate = vatRate.Rate;
                product.VatRateId = request.VatRateId;
            }
            else
            {
                product.VatRate = null;
                product.VatRateId = null;
            }

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductVatRateUpdated
            {
                ProductId = product.Id,
                NewVatRate = product.VatRate
            });

            return Result.Success;
        }
    }
}