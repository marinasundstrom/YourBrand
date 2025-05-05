using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.SubscriptionPlans.SubscriptionPlans;

public sealed record GetProductSubscriptionPlans(string OrganizationId, string? StoreId = null, string? ProductIdOrHandle = null, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<ProductSubscriptionPlanDto>>
{
    public sealed class Handler(CatalogContext catalogContext = default!) : IRequestHandler<GetProductSubscriptionPlans, PagedResult<ProductSubscriptionPlanDto>>
    {
        public async Task<PagedResult<ProductSubscriptionPlanDto>> Handle(GetProductSubscriptionPlans request, CancellationToken cancellationToken)
        {
            var query = catalogContext.ProductSubscriptionPlan
                        .Include(x => x.Product)
                        .ThenInclude(x => x.Prices)
                        .InOrganization(request.OrganizationId)
                        .AsSplitQuery()
                        .AsNoTracking()
                        .AsQueryable()
                        .TagWith(nameof(GetProductSubscriptionPlans));

            /*
            if (request.StoreId is not null)
            {
                query = query.Where(x => x.StoreId == request.StoreId);
            }
            */

            if (!string.IsNullOrEmpty(request.ProductIdOrHandle))
            {
                bool isProductId = long.TryParse(request.ProductIdOrHandle, out var productId);

                query = isProductId
                            ? query.Where(x => x!.ProductId == productId)
                            : query.Where(x =>
                                x.Product!.Handle.StartsWith(request.ProductIdOrHandle));
            }

            var total = await query.CountAsync(cancellationToken);

            if (request.SortBy is null || request.SortDirection is null)
            {
                query = query.OrderBy(x => x.Name);
            }
            else
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }

            var products = await query
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<ProductSubscriptionPlanDto>(products.Select(x => x.ToDto()), total);
        }
    }
}
