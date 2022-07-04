using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveDiscountFromOrderItemCommandHandler : IConsumer<RemoveDiscountFromOrderItemCommand>
{
    private readonly ILogger<RemoveDiscountFromOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveDiscountFromOrderItemCommandHandler(
        ILogger<RemoveDiscountFromOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveDiscountFromOrderItemCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
         .IncludeAll()
         .Where(c => c.OrderNo == message.OrderNo)
         .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new Exception();
        }

        var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

        if (orderItem is null)
        {
            throw new Exception();
        }

        var discount = orderItem.Discounts.FirstOrDefault(x => x.Id == message.DiscountId);

        if (discount is null)
        {
            throw new Exception();
        }

        orderItem.Discounts.Remove(discount);

        context.OrderDiscounts.Remove(discount);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<RemoveDiscountFromOrderItemCommandResponse>(new RemoveDiscountFromOrderItemCommandResponse());
    }
}
