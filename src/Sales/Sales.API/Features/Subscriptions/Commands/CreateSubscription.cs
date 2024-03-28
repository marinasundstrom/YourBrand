using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Persistence;
using YourBrand.Sales.Contracts;
using YourBrand.Sales.Domain.Entities;

using static YourBrand.Sales.Features.Subscriptions.Mappings;

namespace YourBrand.Sales.Features.Subscriptions;

public record CreateSubscription(string ProductId, Guid SubscriptionPlanId, string CustomerId, string? Notes) : IRequest
{
    public class Handler(SalesContext salesContext, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<CreateSubscription>
    {
        public async Task Handle(CreateSubscription request, CancellationToken cancellationToken)
        {
            /*
            var subscriptionPlan = await  salesContext.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            var subscription = new Subscription() 
            {
                SubscriptionPlan = subscriptionPlan!,
                
            };

            var order = new Order()
            {
                Customer = new Customer {
                    Id = request.CustomerId
                },
                Subscription = subscription
            };

            order.AddItem(); */

            var order = new Order();

            await salesContext.SaveChangesAsync();
        }
    }
}