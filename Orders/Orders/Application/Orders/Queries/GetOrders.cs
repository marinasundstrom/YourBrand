using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrdersQueryHandler : IConsumer<GetOrdersQuery>
{
    private readonly ILogger<GetOrdersQueryHandler> _logger;
    private readonly OrdersContext context;

    public GetOrdersQueryHandler(
        ILogger<GetOrdersQueryHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrdersQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var query = context.Orders
            .IncludeAll(
                includeItems: message.IncludeItems,
                includeDiscounts: message.IncludeDiscounts,
                includeCharges: message.IncludeCharges
            )
            .OrderBy(o => o.OrderNo)
            .AsSplitQuery()
            .AsNoTracking();

        var total = await query.CountAsync();

        var orders = await query
            .Skip(message.Skip)
            .Take(message.Limit)
            .ToArrayAsync();

        var response = new GetOrdersQueryResponse()
        {
            Orders = orders.Select(Mappings.CreateOrderDto),
            Total = total
        };

        await consumeContext.RespondAsync<GetOrdersQueryResponse>(response);
    }
}
