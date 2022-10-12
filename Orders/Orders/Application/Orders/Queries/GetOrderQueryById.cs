using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Contracts;

using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public Guid Id { get; set; }

    public class GetOrderQueryByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
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

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .IncludeAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == message.Id);

            if (order is null)
            {
                throw new Exception();
            }

            return Mappings.CreateOrderDto(order);
        }
    }
}