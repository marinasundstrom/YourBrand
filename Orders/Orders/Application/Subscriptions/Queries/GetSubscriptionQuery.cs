using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Subscriptions
{
    public class GetSubscriptionQuery : IRequest
    {
        public GetSubscriptionQuery(Guid subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }

        public Guid SubscriptionId { get; }

        public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery>
        {
            private readonly OrdersContext salesContext;

            public GetSubscriptionQueryHandler(OrdersContext salesContext)
            {
                this.salesContext = salesContext;
            }

            public async Task<Unit> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
            {


                var subscription = await salesContext.Subscriptions
                    .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

                if (subscription is null)
                {
                    throw new Exception();
                }

                return Unit.Value;
            }
        }
    }
}