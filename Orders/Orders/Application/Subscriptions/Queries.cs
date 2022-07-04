using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

using static YourBrand.Orders.Application.Subscriptions.Mappings;

namespace YourBrand.Orders.Application.Subscriptions
{
    public class GetSubscriptionsQueryHandler : IConsumer<GetSubscriptionsQuery>
    {
        private readonly OrdersContext salesContext;

        public GetSubscriptionsQueryHandler(OrdersContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<GetSubscriptionsQuery> consumeContext)
        {
            var subscriptions = await salesContext.Subscriptions
                .AsNoTracking()
                .ToListAsync();

            var response = new GetSubscriptionsQueryResponse
            {
                Subscriptions = subscriptions.Select(Map)
            };

            await consumeContext.RespondAsync<GetSubscriptionsQueryResponse>(response);
        }
    }

    public class GetSubscriptionQueryHandler : IConsumer<GetSubscriptionQuery>
    {
        private readonly OrdersContext salesContext;

        public GetSubscriptionQueryHandler(OrdersContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task Consume(ConsumeContext<GetSubscriptionQuery> consumeContext)
        {
            var request = consumeContext.Message;

            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new Exception();
            }

            await consumeContext.RespondAsync<SubscriptionDto>(Map(subscription));
        }
    }
}