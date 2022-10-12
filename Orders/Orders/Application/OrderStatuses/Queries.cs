using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.OrderStatuses
{
    public class GetOrderStatusesQuery : IRequest<GetOrderStatusesQueryResponse>
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;

        public class GetOrderStatusesQueryHandler : IRequestHandler<GetOrderStatusesQuery, GetOrderStatusesQueryResponse>
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

            public async Task<GetOrderStatusesQueryResponse> Handle(GetOrderStatusesQuery request, CancellationToken cancellationToken)
            {
                var message = request;

                var query = context.OrderStatuses
                    .AsSplitQuery()
                    .AsNoTracking();

                var total = await query.CountAsync();

                var orderStatuses = await query
                    .Skip(message.Skip)
                    .Take(message.Limit)
                    .ToArrayAsync();

                return new GetOrderStatusesQueryResponse()
                {
                    OrderStatuses = orderStatuses.Select(Mappings.CreateOrderStatusDto),
                    Total = total
                };
            }
        }
    }
}

public class GetOrderStatusesQueryResponse
{
    public IEnumerable<OrderStatusDto> OrderStatuses { get; set; } = null!;

    public int Total { get; set; }
}