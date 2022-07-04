using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Products.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.OrderStatuses
{
    public class GetOrderStatusesQueryHandler : IConsumer<GetOrderStatusesQuery>
    {
        private readonly ILogger<GetOrderStatusesQueryHandler> _logger;
        private readonly OrdersContext context;

        public GetOrderStatusesQueryHandler(
            ILogger<GetOrderStatusesQueryHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task Consume(ConsumeContext<GetOrderStatusesQuery> consumeContext)
        {
            var message = consumeContext.Message;

            var query = context.OrderStatuses
                .AsSplitQuery()
                .AsNoTracking();

            var total = await query.CountAsync();

            var orderStatuses = await query
                .Skip(message.Skip)
                .Take(message.Limit)
                .ToArrayAsync();

            var response = new GetOrderStatusesQueryResponse()
            {
                OrderStatuses = orderStatuses.Select(Mappings.CreateOrderStatusDto),
                Total = total
            };

            await consumeContext.RespondAsync<GetOrderStatusesQueryResponse>(response);
        }
    }
}