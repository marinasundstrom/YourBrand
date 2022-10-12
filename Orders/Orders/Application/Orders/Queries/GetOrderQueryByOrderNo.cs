using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public record GetOrderByOrderNoQuery(int OrderNo) : IRequest<OrderDto>
{
    public bool IncludeItems { get; set; } = true;

    public bool IncludeDiscounts { get; set; } = true;

    public bool IncludeCharges { get; set; } = true;

    public class GetOrderQueryByOrderNoHandler : IRequestHandler<GetOrderByOrderNoQuery, OrderDto>
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

        public async Task<OrderDto> Handle(GetOrderByOrderNoQuery request, CancellationToken cancellationToken)
        {
            var message = request;

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

            return Mappings.CreateOrderDto(order);
        }
    }
}