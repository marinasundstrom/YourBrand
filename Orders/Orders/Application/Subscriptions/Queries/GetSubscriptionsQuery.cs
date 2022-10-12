using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

using static YourBrand.Orders.Application.Subscriptions.Mappings;

namespace YourBrand.Orders.Application.Subscriptions
{
    public class GetSubscriptionsQuery : IRequest<GetSubscriptionsQueryResponse>
    {
        public class GetSubscriptionsQueryHandler : IRequestHandler<GetSubscriptionsQuery, GetSubscriptionsQueryResponse>
        {
            private readonly OrdersContext salesContext;

            public GetSubscriptionsQueryHandler(OrdersContext salesContext)
            {
                this.salesContext = salesContext;
            }

            public async Task<GetSubscriptionsQueryResponse> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
            {
                var subscriptions = await salesContext.Subscriptions
                    .AsNoTracking()
                    .ToListAsync();

                return new GetSubscriptionsQueryResponse
                {
                    Subscriptions = subscriptions.Select(Map)
                };
            }
        }
    }

    public class GetSubscriptionsQueryResponse
    {
        public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = null!;
    }
}