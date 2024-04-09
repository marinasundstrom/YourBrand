using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Models;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions.Plans;

public record GetSubscriptionPlansQuery : IRequest<PagedResult<SubscriptionPlanDto>>
{
    public class Handler(SalesContext salesContext) : IRequestHandler<GetSubscriptionPlansQuery, PagedResult<SubscriptionPlanDto>>
    {
        public async Task<PagedResult<SubscriptionPlanDto>> Handle(GetSubscriptionPlansQuery request, CancellationToken cancellationToken)
        {
            var subscriptionPlans = await salesContext.SubscriptionPlans
                .AsNoTracking()
                .ToListAsync();

            var count = subscriptionPlans.Count;

            return new PagedResult<SubscriptionPlanDto>(subscriptionPlans.Select(Mappings.ToDto), count);
        }
    }
}