using MassTransit;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public class UpdateOrderItemQuantityCommandHandler : IConsumer<UpdateOrderItemQuantityCommand>
{
    private readonly ILogger<UpdateOrderItemQuantityCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public UpdateOrderItemQuantityCommandHandler(
        ILogger<UpdateOrderItemQuantityCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<UpdateOrderItemQuantityCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = context.Orders
            .Where(c => c.OrderNo == message.OrderNo)
            .IncludeAll()
            .FirstOrDefault();

        if (order is null)
        {
            throw new Exception();
        }

        var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

        if (item is null)
        {
            throw new Exception();
        }

        var oldQuantity = item.Quantity;

        item.UpdateQuantity(message.Quantity);

        order.Update();

        await context.SaveChangesAsync();

        await bus.Publish(new OrderItemQuantityUpdatedEvent(order.OrderNo, item.Id, oldQuantity, item.Quantity));

        await consumeContext.RespondAsync<UpdateOrderItemQuantityCommandResponse>(new UpdateOrderItemQuantityCommandResponse(order.OrderNo, item.Id));
    }
}
