using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class PlaceOrderCommandHandler : IConsumer<PlaceOrderCommand>
{
    private readonly ILogger<PlaceOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;
    private readonly SubscriptionOrderGenerator subscriptionOrderGenerator;

    public PlaceOrderCommandHandler(
        ILogger<PlaceOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus,
        SubscriptionOrderGenerator subscriptionOrderGenerator)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
        this.subscriptionOrderGenerator = subscriptionOrderGenerator;
    }

    public async Task Consume(ConsumeContext<PlaceOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

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

        await consumeContext.RespondAsync<PlaceOrderCommandResponse>(new PlaceOrderCommandResponse());
    }
}
