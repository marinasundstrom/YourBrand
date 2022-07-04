using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class AddDiscountToOrderCommandHandler : IConsumer<AddDiscountToOrderCommand>
{
    private readonly ILogger<AddDiscountToOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public AddDiscountToOrderCommandHandler(
        ILogger<AddDiscountToOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddDiscountToOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var details = message.DiscountDetails;

        var order = await context.Orders
             .IncludeAll()
             .Where(c => c.OrderNo == message.OrderNo)
             .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new Exception();
        }

        if (details.Percent is not null)
        {
            if (order.Discounts.Any(x => x.Percent is null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            if (order.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot add another discount based on percenOrderNoe.");
            }
        }

        if (details.Percent is null && order.Discounts.Any(x => x.Percent is not null))
        {
            throw new Exception("Cannot combine different discount types.");
        }

        var discount = new OrderDiscount
        {
            Id = Guid.NewGuid(),
            Order = order,
            Amount = details.Amount * -1,
            Percent = details.Percent * -1,
            Description = details.Description!,
            DiscountId = details.DiscountId
        };

        order.Discounts.Add(discount);

        context.OrderDiscounts.Add(discount);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<AddDiscountToOrderCommandResponse>(new AddDiscountToOrderCommandResponse());
    }
}
