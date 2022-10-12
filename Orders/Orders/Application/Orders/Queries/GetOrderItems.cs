using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public record GetOrderItemsQuery(int OrderNo) : IRequest<IEnumerable<OrderItemDto>>
{    
    public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, IEnumerable<OrderItemDto>>
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

        public async Task<IEnumerable<OrderItemDto>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
        {
            var message = request;
            var order = await context.Orders
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            return order.Items.Select(Mappings.CreateOrderItemDto);
        }
    }
}