using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderQueryByOrderNoHandler : IConsumer<GetOrderByOrderNoQuery>
{
    private readonly ILogger<GetOrderQueryByOrderNoHandler> _logger;
    private readonly OrdersContext context;

    public GetOrderQueryByOrderNoHandler(
        ILogger<GetOrderQueryByOrderNoHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrderByOrderNoQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll(
                includeItems: message.IncludeItems,
                includeDiscounts: message.IncludeDiscounts,
                includeCharges: message.IncludeCharges
            )
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

        if (order is null)
        {
            throw new Exception();
        }

        var dto = Mappings.CreateOrderDto(order);

        await consumeContext.RespondAsync<OrderDto>(dto);
    }
}
