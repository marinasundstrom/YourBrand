using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderItemQuery : IRequest<OrderItemDto>
{
    public int OrderNo { get; set; }

    public Guid OrderItemId { get; set; }

    public class GetOrderItemQueryHandler : IRequestHandler<GetOrderItemQuery, OrderItemDto>
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

        public async Task<OrderItemDto> Handle(GetOrderItemQuery request, CancellationToken cancellationToken)
        {
            var message = request;

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

            return Mappings.CreateOrderItemDto(item);
        }
    }
}