using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public record GetOrdersQuery : IRequest<GetOrdersQueryResponse>
{
    public int Skip { get; set; }

    public int Limit { get; set; } = 10;

    public bool IncludeItems { get; set; } = true;

    public bool IncludeDiscounts { get; set; } = true;

    public bool IncludeCharges { get; set; } = true;

    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, GetOrdersQueryResponse>
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

        public async Task<GetOrdersQueryResponse> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var message = request;

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

            return new GetOrdersQueryResponse()
            {
                Orders = orders.Select(Mappings.CreateOrderDto),
                Total = total
            };
        }
    }
}

public class GetOrdersQueryResponse
{
    public IEnumerable<OrderDto> Orders { get; set; } = null!;

    public int Total { get; set; }
}
