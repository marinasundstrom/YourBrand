using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveChargeFromOrderCommandHandler : IConsumer<RemoveChargeFromOrderCommand>
{
    private readonly ILogger<RemoveChargeFromOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveChargeFromOrderCommandHandler(
        ILogger<RemoveChargeFromOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveChargeFromOrderCommand> consumeContext)
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

        var Charge = order.Charges.FirstOrDefault(x => x.Id == message.ChargeId);

        if (Charge is null)
        {
            throw new Exception();
        }

        order.Charges.Remove(Charge);

        context.OrderCharges.Remove(Charge);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<RemoveChargeFromOrderCommandResponse>(new RemoveChargeFromOrderCommandResponse());
    }
}
