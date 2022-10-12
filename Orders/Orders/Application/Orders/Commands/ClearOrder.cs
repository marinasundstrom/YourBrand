using MediatR;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class ClearOrderCommand : IRequest
{
    public ClearOrderCommand(int orderNo)
    {
        OrderNo = orderNo;
    }

    public int OrderNo { get; }

    public class ClearOrderCommandHandler : IRequestHandler<ClearOrderCommand>
    {
        private readonly ILogger<ClearOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public ClearOrderCommandHandler(
            ILogger<ClearOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(ClearOrderCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            order.Clear();

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}