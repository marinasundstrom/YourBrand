using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveChargeFromOrderItemCommandHandler : IConsumer<RemoveChargeFromOrderItemCommand>
{
    private readonly ILogger<RemoveChargeFromOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveChargeFromOrderItemCommandHandler(
        ILogger<RemoveChargeFromOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveChargeFromOrderItemCommand> consumeContext)
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

        var Charge = orderItem.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

        if (Charge is null)
        {
            throw new Exception();
        }

        orderItem.Charges.Remove(Charge);

        context.OrderCharges.Remove(Charge);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<RemoveChargeFromOrderItemCommandResponse>(new RemoveChargeFromOrderItemCommandResponse());
    }
}