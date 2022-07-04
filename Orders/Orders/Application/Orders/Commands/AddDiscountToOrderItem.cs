using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class AddDiscountToOrderItemCommandHandler : IConsumer<AddDiscountToOrderItemCommand>
{
    private readonly ILogger<AddDiscountToOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public AddDiscountToOrderItemCommandHandler(
        ILogger<AddDiscountToOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddDiscountToOrderItemCommand> consumeContext)
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

        var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

        if (orderItem is null)
        {
            throw new Exception();
        }

        if (details.Percent is not null)
        {
            if (orderItem.Discounts.Any(x => x.Percent is null))
            {
                throw new Exception("Cannot combine different discount types.");
            }

            if (orderItem.Discounts.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot add another discount based on percenOrderNoe.");
            }
        }

        if (details.Percent is null && orderItem.Discounts.Any(x => x.Percent is not null))
        {
            throw new Exception("Cannot combine different discount types.");
        }

        var discount = new OrderDiscount
        {
            Id = Guid.NewGuid(),
            OrderItem = orderItem,
            Amount = details.Amount * -1,
            Percent = details.Percent * -1,
            Description = details.Description!,
            DiscountId = details.DiscountId
        };

        orderItem.Discounts.Add(discount);

        context.OrderDiscounts.Add(discount);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<AddDiscountToOrderItemCommandResponse>(new AddDiscountToOrderItemCommandResponse());
    }
}
