using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record CalculateProductPrice(
    string OrganizationId,
    string IdOrHandle,
    List<ProductOptionValue> OptionValues,
    int Quantity,
    string? SubscriptionPlanId = null) : IRequest<ProductPriceResult>
{
    public sealed class Handler(CatalogContext catalogContext, ProductPricingService productPricingService) : IRequestHandler<CalculateProductPrice, ProductPriceResult>
    {
        public async Task<ProductPriceResult> Handle(CalculateProductPrice request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var query = catalogContext.Products
                .InOrganization(request.OrganizationId)
                .IncludeAll()
                .Include(x => x.Prices)
                .AsSplitQuery()
                .AsQueryable();

            var product = isId ?
                await query.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await query.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            ProductSubscriptionPlan? subscriptionPlan = null;

            if (request.SubscriptionPlanId is not null)
            {
                subscriptionPlan = product.SubscriptionPlans.FirstOrDefault(x => x.Id == request.SubscriptionPlanId);
            }

            return productPricingService.CalculatePrice(product, request.OptionValues, subscriptionPlan, request.Quantity);
        }
    }
}
