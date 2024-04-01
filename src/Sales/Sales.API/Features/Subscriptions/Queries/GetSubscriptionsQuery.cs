using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Models;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions;

public record GetSubscriptionsQuery : IRequest<PagedResult<SubscriptionDto>>
{
    public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, PagedResult<SubscriptionDto>>
    {
        private readonly SalesContext salesContext;

        public GetSubscriptionsQueryHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task<PagedResult<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await salesContext.Subscriptions
                .Include(x => x.SubscriptionPlan)
                .Include(x => x.Order)
                .AsNoTracking()
                .ToListAsync();

            var count = subscriptions.Count;

            return new PagedResult<SubscriptionDto>(subscriptions.Select(Mappings.ToDto), count);
        }
    }
}