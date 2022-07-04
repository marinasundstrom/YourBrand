using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderTotalsQueryHandler : IConsumer<GetOrderTotalsQuery>
{
    private readonly ILogger<GetOrderTotalsQueryHandler> _logger;
    private readonly OrdersContext context;

    public GetOrderTotalsQueryHandler(
        ILogger<GetOrderTotalsQueryHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrderTotalsQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll()
            .Where(c => c.OrderNo == message.OrderNo)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new Exception();
        }

        var dto = new OrderTotalsDto()
        {
            Totals = order.Vat(),
            SubTotal = order.Items.Sum(i => i.SubTotal()),
            Vat = order.Items.Sum(i => i.Vat() * (decimal)i.Quantity),
            Discounts = order.Discounts.Select(Mappings.CreateOrderDiscountDto),
            Rounding = order.Rounding(),
            Total = order.Total(true)
        };

        await consumeContext.RespondAsync<OrderTotalsDto>(dto);
    }
}
