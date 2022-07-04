using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderItemsQueryHandler : IConsumer<GetOrderItemsQuery>
{
    private readonly ILogger<GetOrderItemsQueryHandler> _logger;
    private readonly OrdersContext context;

    public GetOrderItemsQueryHandler(
        ILogger<GetOrderItemsQueryHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrderItemsQuery> consumeContext)
    {
        var message = consumeContext.Message;
        var order = await context.Orders
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (order is null)
        {
            throw new Exception();
        }

        var dto = new GetOrderItemsQueryResponse
        {
            OrderItems = order.Items.Select(Mappings.CreateOrderItemDto)
        };

        await consumeContext.RespondAsync<GetOrderItemsQueryResponse>(dto);
    }
}
