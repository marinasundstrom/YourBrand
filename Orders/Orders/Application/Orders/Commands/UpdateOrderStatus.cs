using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class UpdateOrderStatusCommandHandler : IConsumer<UpdateOrderStatusCommand>
{
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public UpdateOrderStatusCommandHandler(
        ILogger<UpdateOrderStatusCommandHandler> logger,
        OrdersContext context,
        IBus bus,
        SubscriptionOrderGenerator subscriptionOrderGenerator)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<UpdateOrderStatusCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll()
            .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

        if (order is null)
        {
            throw new Exception();
        }

        order.UpdateOrderStatus(message.OrderStatusId);


        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<UpdateOrderStatusCommandResponse>(new UpdateOrderStatusCommandResponse());
    }
}
