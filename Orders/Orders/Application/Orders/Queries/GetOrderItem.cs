using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderItemQueryHandler : IConsumer<GetOrderItemQuery>
{
    private readonly ILogger<GetOrderItemQueryHandler> _logger;
    private readonly OrdersContext context;

    public GetOrderItemQueryHandler(
        ILogger<GetOrderItemQueryHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrderItemQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

        if (order is null)
        {
            order = new Order();
        }

        var item = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

        if (item is null)
        {
            throw new Exception();
        }

        var dto = Mappings.CreateOrderItemDto(item);

        await consumeContext.RespondAsync<OrderItemDto>(dto);
    }
}
