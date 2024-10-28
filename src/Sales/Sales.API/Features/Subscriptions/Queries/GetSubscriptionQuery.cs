using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record GetSubscriptionQuery(Guid SubscriptionId) : IRequest<SubscriptionDto>
{

    public class GetSubscriptionQueryHandler(SalesContext salesContext) : IRequestHandler<GetSubscriptionQuery, SubscriptionDto>
    {
        public async Task<SubscriptionDto> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                .Include(x => x.Type)
                .Include(x => x.Status)
                .Include(x => x.Plan)
                .Include(x => x.Order)
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionId);

            if (subscription is null)
            {
                throw new Exception();
            }

            return subscription.ToDto();
        }
    }
}