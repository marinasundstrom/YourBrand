using MassTransit;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveOrderItemCommandHandler : IConsumer<RemoveOrderItemCommand>
{
    private readonly ILogger<RemoveOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveOrderItemCommandHandler(
        ILogger<RemoveOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveOrderItemCommand> consumeContext)
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

        item.Clear();

        order.Items.Remove(item);

        order.Update();

        await context.SaveChangesAsync();

        item.AddDomainEvent(new OrderItemRemovedEvent(order.OrderNo, item.Id));

        await consumeContext.RespondAsync<RemoveOrderItemCommandResponse>(new RemoveOrderItemCommandResponse(order.OrderNo, item.Id));
    }
}
