using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Models;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record GetSubscriptionsQuery : IRequest<PagedResult<SubscriptionDto>>
{
    public class GetSubscriptionsQueryHandler(SalesContext salesContext) : IRequestHandler<GetSubscriptionsQuery, PagedResult<SubscriptionDto>>
    {
        public async Task<PagedResult<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await salesContext.Subscriptions
                .Include(x => x.Type)
                .Include(x => x.Status)
                .Include(x => x.SubscriptionPlan)
                .Include(x => x.Order)
                .AsNoTracking()
                .ToListAsync();

            var count = subscriptions.Count;

            return new PagedResult<SubscriptionDto>(subscriptions.Select(Mappings.ToDto), count);
        }
    }
}