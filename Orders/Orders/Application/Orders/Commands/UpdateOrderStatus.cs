using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders.Commands;

public class UpdateOrderStatusCommand : IRequest
{
    public UpdateOrderStatusCommand(int orderNo, string orderStatusId)
    {
        OrderNo = orderNo;
        OrderStatusId = orderStatusId;
    }

    public int OrderNo { get; }

    public string OrderStatusId { get; }
    
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public UpdateOrderStatusCommandHandler(
            ILogger<UpdateOrderStatusCommandHandler> logger,
            OrdersContext context,
            SubscriptionOrderGenerator subscriptionOrderGenerator)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(c => c.OrderNo == message.OrderNo);

            if (order is null)
            {
                throw new Exception();
            }

            order.UpdateOrderStatus(message.OrderStatusId);


            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}