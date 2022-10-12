using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public record PlaceOrderCommand(int OrderNo) : IRequest
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand>
    {
        private readonly ILogger<PlaceOrderCommandHandler> _logger;
        private readonly OrdersContext context;
         private readonly SubscriptionOrderGenerator subscriptionOrderGenerator;

        public PlaceOrderCommandHandler(
            ILogger<PlaceOrderCommandHandler> logger,
            OrdersContext context,
            SubscriptionOrderGenerator subscriptionOrderGenerator)
        {
            _logger = logger;
            this.context = context;
            this.subscriptionOrderGenerator = subscriptionOrderGenerator;
        }

        public async Task<Unit> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            var orders = subscriptionOrderGenerator.GetOrders(order);

            if (orders.Any())
            {
                foreach (var o in orders)
                {
                    o.Update();
                    context.Orders.Add(o);
                }

                await context.SaveChangesAsync();
            }

            return Unit.Value;
        }
    }
}