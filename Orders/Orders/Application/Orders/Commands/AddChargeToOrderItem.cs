using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class AddChargeToOrderItemCommandHandler : IConsumer<AddChargeToOrderItemCommand>
{
    private readonly ILogger<AddChargeToOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public AddChargeToOrderItemCommandHandler(
        ILogger<AddChargeToOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddChargeToOrderItemCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var details = message.ChargeDetails;

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
            if (orderItem.Charges.Any(x => x.Percent is null))
            {
                throw new Exception("Cannot combine different Charge types.");
            }

            if (orderItem.Charges.Any(x => x.Percent is not null))
            {
                throw new Exception("Cannot add another Charge based on percenOrderNoe.");
            }
        }

        if (details.Percent is null && orderItem.Charges.Any(x => x.Percent is not null))
        {
            throw new Exception("Cannot combine different Charge types.");
        }

        var Charge = new OrderCharge
        {
            Id = Guid.NewGuid(),
            OrderItem = orderItem,
            Amount = details.Amount,
            Percent = details.Percent,
            Description = details.Description!,
            ChargeId = details.ChargeId
        };

        orderItem.Charges.Add(Charge);

        context.OrderCharges.Add(Charge);

        order.Update();

        await context.SaveChangesAsync();

        await consumeContext.RespondAsync<AddChargeToOrderItemCommandResponse>(new AddChargeToOrderItemCommandResponse());
    }
}
