using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.Subscriptions;
using YourBrand.Sales.API.Models;
using YourBrand.Sales.Domain.Entities;

using static YourBrand.Sales.Features.Subscriptions.Mappings;
using YourBrand.Sales.API.Persistence;

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
                .AsNoTracking()
                .ToListAsync();

            var count = subscriptions.Count;

            return new PagedResult<SubscriptionDto>(subscriptions.Select(Mappings.ToDto), count);
        }
    }
}