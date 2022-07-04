using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Products.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class QueryOrdersByCustomFieldValueQueryHandler : IConsumer<QueryOrdersByCustomFieldValueQuery>
{
    private readonly ILogger<QueryOrdersByCustomFieldValueQueryHandler> _logger;
    private readonly OrdersContext context;

    public QueryOrdersByCustomFieldValueQueryHandler(
        ILogger<QueryOrdersByCustomFieldValueQueryHandler> logger,
        OrdersContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public async Task Consume(ConsumeContext<QueryOrdersByCustomFieldValueQuery> consumeContext)
    {
        var message = consumeContext.Message;

        var orders = await context.Orders
            .IncludeAll()
            .Where(c => c.CustomFields.Any(m => m.CustomFieldId == message.CustomFieldId && m.Value == message.Value))
            .AsNoTracking()
            .ToArrayAsync();

        var dtos = orders.Select(Mappings.CreateOrderDto);

        await consumeContext.RespondAsync<QueryOrdersByCustomFieldValueQueryResponse>(new QueryOrdersByCustomFieldValueQueryResponse
        {
            Orders = dtos
        });
    }
}