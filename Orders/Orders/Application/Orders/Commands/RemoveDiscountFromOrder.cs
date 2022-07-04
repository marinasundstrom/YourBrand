using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class RemoveDiscountFromOrderCommandHandler : IConsumer<RemoveDiscountFromOrderCommand>
{
    private readonly ILogger<RemoveDiscountFromOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveDiscountFromOrderCommandHandler(
        ILogger<RemoveDiscountFromOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveDiscountFromOrderCommand> consumeContext)
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

        var discount = order.Discounts.FirstOrDefault(x => x.Id == message.DiscountId);

        if (discount is null)
        {
            throw new Exception();
        }

        order.Discounts.Remove(discount);

        context.OrderDiscounts.Remove(discount);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<RemoveDiscountFromOrderCommandResponse>(new RemoveDiscountFromOrderCommandResponse());
    }
}
