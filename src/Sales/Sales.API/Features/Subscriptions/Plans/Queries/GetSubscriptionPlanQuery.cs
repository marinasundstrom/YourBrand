using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions.Plans;

public record GetSubscriptionPlanQuery(Guid SubscriptionPlanId) : IRequest<SubscriptionPlanDto>
{
    public class Handler : IRequestHandler<GetSubscriptionPlanQuery, SubscriptionPlanDto>
    {
        private readonly SalesContext salesContext;

        public Handler(SalesContext salesContext)
        {
            this.salesContext = salesContext;
        }

        public async Task<SubscriptionPlanDto> Handle(GetSubscriptionPlanQuery request, CancellationToken cancellationToken)
        {
            var subscriptionPlan = await salesContext.SubscriptionPlans
                .FirstOrDefaultAsync(c => c.Id == request.SubscriptionPlanId);

            if (subscriptionPlan is null)
            {
                throw new Exception();
            }

            return subscriptionPlan.ToDto();
        }
    }
}