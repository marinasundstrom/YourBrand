using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement.Plans;

public record GetSubscriptionPlanQuery(Guid SubscriptionPlanId) : IRequest<SubscriptionPlanDto>
{
    public class Handler(SalesContext salesContext) : IRequestHandler<GetSubscriptionPlanQuery, SubscriptionPlanDto>
    {
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