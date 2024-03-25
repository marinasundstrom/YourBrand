using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.API.Persistence;

namespace YourBrand.Sales.Features.Subscriptions;

public record GetSubscriptionQuery(Guid SubscriptionId) : IRequest<SubscriptionDto>
{

    public class GetSubscriptionQueryHandler : IRequestHandler<GetSubscriptionQuery, SubscriptionDto>
    {
        private readonly SalesContext salesContext;

        public GetSubscriptionQueryHandler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task<SubscriptionDto> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                .Include(x => x.SubscriptionPlan)
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new Exception();
            }

            return subscription.ToDto();
        }
    }
}