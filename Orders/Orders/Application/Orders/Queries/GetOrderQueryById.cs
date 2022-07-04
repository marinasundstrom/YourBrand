using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderQueryByIdHandler : IConsumer<GetOrderByIdQuery>
{
    private readonly ILogger<GetOrderQueryByIdHandler> _logger;
    private readonly OrdersContext context;

    public GetOrderQueryByIdHandler(
        ILogger<GetOrderQueryByIdHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<GetOrderByIdQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var order = await context.Orders
            .IncludeAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == message.Id);

        if (order is null)
        {
            throw new Exception();
        }

        var dto = Mappings.CreateOrderDto(order);

        await consumeContext.RespondAsync<OrderDto>(dto);
    }
}
