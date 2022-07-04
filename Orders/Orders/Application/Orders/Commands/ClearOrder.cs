using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class ClearOrderCommandHandler : IConsumer<ClearOrderCommand>
{
    private readonly ILogger<ClearOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public ClearOrderCommandHandler(
        ILogger<ClearOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<ClearOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll()
            .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

        if (order is null)
        {
            throw new Exception();
        }

        order.Clear();

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<ClearOrderCommandResponse>(new ClearOrderCommandResponse(order.OrderNo));
    }
}
