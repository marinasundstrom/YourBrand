using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.Subscriptions;
using YourBrand.Sales.Models;
using YourBrand.Sales.Domain.Entities;

using static YourBrand.Sales.Features.Subscriptions.Mappings;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions.Plans;

public record GetSubscriptionPlansQuery : IRequest<PagedResult<SubscriptionPlanDto>>
{
    public class Handler : IRequestHandler<GetSubscriptionPlansQuery, PagedResult<SubscriptionPlanDto>>
    {
        private readonly SalesContext salesContext;

        public Handler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

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